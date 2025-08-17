using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Account;
using HangOut.Domain.Payload.Response.Account;
using HangOut.Domain.Payload.Settings;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.Extensions.Options;

namespace HangOut.API.Services.Implement;

public class AccountService : BaseService<AccountService>, IAccountService
{
    private readonly IUploadService _uploadService;
    private readonly IRedisService _redisService;
    private readonly JwtSettings _jwtSettings;
    public AccountService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor,
        IUploadService uploadService, IRedisService redisService, IOptions<JwtSettings> jwtSettings) : base(unitOfWork, logger, httpContextAccessor)
    {
        _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
        _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterUserAsync(RegisterRequest request)
    {
        var accountList = await _unitOfWork.GetRepository<Account>().GetListAsync();
        if (accountList.Any(x => x.Email.Equals(request.Email)))
            throw new BadHttpRequestException("Email đã được sử dụng");
        if (string.IsNullOrEmpty(request.Phone))
        {
            if (accountList.Any(x => x.Phone != null && x.Phone.Equals(request.Phone)))
                throw new BadHttpRequestException("Số điện thoại đã được sử dụng");
        }
        
        var key = "otp:" + request.Email;
        var existingOtp = await _redisService.GetStringAsync(key);

        if (string.IsNullOrEmpty(existingOtp))
            throw new BadHttpRequestException("Không tìm thấy mã OTP");
        if (!existingOtp.Equals(request.Otp))
            throw new BadHttpRequestException("Mã OTP không chính xác");
        
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Phone = request.Phone,
            Password = PasswordUtil.HashPassword(request.Password),
            Active = true,
            Role = ERoleEnum.User,
        };
        var user = new User()
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Name = request.Name,
            Avatar = request.AvatarImage != null ? await _uploadService.UploadImageAsync(request.AvatarImage) : null,
            Active = true
        };
        await _unitOfWork.GetRepository<User>().InsertAsync(user);
        await _unitOfWork.GetRepository<Account>().InsertAsync(account);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Một lỗi đã xảy ra trong quá trình đăng ký tài khoản");
        var registerResponse = new RegisterResponse() 
        {
            AccessToken = JwtUtil.GenerateJwtToken(account, _jwtSettings),
            AccountId = account.Id,
            Phone = account.Phone,
            Email = account.Email,
            Name = user.Name,
            Avatar = user.Avatar,
            Role = account.Role
        };
        return new ApiResponse<RegisterResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Đăng ký tài khoản thành công",
            Data = registerResponse
        };
    }
}