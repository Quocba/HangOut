using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Authentication;
using HangOut.Domain.Payload.Response.Authentication;
using HangOut.Domain.Payload.Settings;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using IAuthenticationService = HangOut.API.Services.Interface.IAuthenticationService;
using LoginRequest = HangOut.Domain.Payload.Request.Authentication.LoginRequest;

namespace HangOut.API.Services.Implement;

public class AuthenticationService : BaseService<AuthenticationService>, IAuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService;
    public AuthenticationService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor,
        IOptions<JwtSettings> jwtSettings, 
        IWebHostEnvironment env, IEmailService emailService, IRedisService redisService) : base(unitOfWork, logger, httpContextAccessor)
    {
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _env = env ?? throw new ArgumentNullException(nameof(env));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: x => (x.Email.Equals(loginRequest.EmailOrPhoneNumber) 
                             || x.Phone.Equals(loginRequest.EmailOrPhoneNumber))
        );
        if(account == null)
            throw new BadHttpRequestException("Tài khoản không tồn tại");
        if(account.Password != PasswordUtil.HashPassword(loginRequest.Password))
            throw new BadHttpRequestException("Tài khoản hoặc mật khẩu không đúng");

        var loginResponse = new LoginResponse()
        {
            Email = account.Email,
            AccountId = account.Id,
            AccessToken = JwtUtil.GenerateJwtToken(account, _jwtSettings),
            Role = account.Role,
            Phone = account.Phone,
        };
        if (account.Role == ERoleEnum.User)
        {
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == account.Id
            );
            if (user == null)
                throw new BadHttpRequestException("Người dùng không tồn tại");
            loginResponse.Name = user.Name;
            loginResponse.Avatar = user.Avatar;
        }
        return new ApiResponse<LoginResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Đăng nhập thành công",
            Data = loginResponse,
        };

    }

    public async Task<ApiResponse> SendOtpRequest(SendOtpRequest sendOtpRequest)
    {
        var key = "otp:" + sendOtpRequest.Email;
        
        var existingOtp = await _redisService.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(existingOtp))
            throw new BadHttpRequestException("Mã OTP đã được gửi");
        
        var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();

        switch (sendOtpRequest.OtpType)
        {
            case EOtpType.Register:
                if (accounts.Any(x => x.Email.Equals(sendOtpRequest.Email)))
                    throw new BadHttpRequestException("Email đã được sử dụng");
                if (sendOtpRequest.Phone != null)
                {
                    if (accounts.Any(x => x.Phone != null && x.Phone.Equals(sendOtpRequest.Phone)))
                        throw new BadHttpRequestException("Số điện thoại đã được sử dụng");
                }
                break;
            case EOtpType.ForgotPassword:
                if (!accounts.Any(x => x.Email.Equals(sendOtpRequest.Email)))
                    throw new BadHttpRequestException("Email không tồn tại");
                break;
            default:
                throw new BadHttpRequestException("Loại mã OTP không hợp lệ");
        }
        
        var otp = OtpUtil.GenerateOtp();
        
        var emailTemplate = GetTemplate(sendOtpRequest.Email, otp);
        var emailMessage = new EmailMessage()
        {
            ToAddress = sendOtpRequest.Email,
            Body = emailTemplate,
            Subject = otp + " là mã xác thực của bạn",
        };
        await _emailService.SendEmailAsync(emailMessage);
        
        var redisSuccess = await _redisService.SetStringAsync(key, otp, TimeSpan.FromMinutes(2));
        
        if (!redisSuccess)
            throw new BadHttpRequestException("Lỗi khi lưu mã OTP");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Gửi mã OTP thành công",
            Data = null
        };
    }

    public async Task<ApiResponse> ForgetPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
    {
        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: x => x.Email.Equals(forgotPasswordRequest.Email)
        );
        if (account == null)
            throw new BadHttpRequestException("Email không tồn tại");
        
        var key = "otp:" + forgotPasswordRequest.Email;
        var existingOtp = await _redisService.GetStringAsync(key);
        if (string.IsNullOrEmpty(existingOtp))
            throw new BadHttpRequestException("Không tìm thấy mã OTP");
        if (!existingOtp.Equals(forgotPasswordRequest.Otp))
            throw new BadHttpRequestException("Mã OTP không chính xác");
        
        account.Password = PasswordUtil.HashPassword(forgotPasswordRequest.NewPassword);
        _unitOfWork.GetRepository<Account>().UpdateAsync(account);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Một lỗi đã xảy ra trong quá trình đặt lại mật khẩu");
        return new ApiResponse() 
        {
            Status = StatusCodes.Status200OK,
            Message = "Đặt lại mật khẩu thành công",
            Data = null
        };
    }

    private string GetTemplate(string email, string otp)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "assets/html", "template.html");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Email template not found", templatePath);
        
        var template = File.ReadAllText(templatePath);
        template = template.Replace("{Email}", email);
        template = template.Replace("{otp}", otp);
        return template;
    }

    public async Task<ApiResponse<string>> ChangePassword(Guid userId,ChangePasswordRequest request)
    {
        try
        {
           var getUser = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == userId);
            if (getUser == null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy người dùng",
                    Data = null
                };
            }

            if (getUser != null && !getUser.Password.Equals(PasswordUtil.HashPassword(request.OldPassword)))
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu cũ không chính xác",
                    Data = null
                };
            }

            getUser.Password = PasswordUtil.HashPassword(request.NewPassword);

            _unitOfWork.GetRepository<Account>().UpdateAsync(getUser);
            await _unitOfWork.CommitAsync();

            return new ApiResponse<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đổi mật khẩu thành công",
                Data = null
            };

        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
    }
}