using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Authentication;
using HangOut.Domain.Payload.Response.Authentication;
using LoginRequest = HangOut.Domain.Payload.Request.Authentication.LoginRequest;

namespace HangOut.API.Services.Interface;

public interface IAuthenticationService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    Task<ApiResponse> SendOtpRequest (SendOtpRequest sendOtpRequest);
    Task<ApiResponse> ForgetPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
    Task<ApiResponse<string>> ChangePassword(Guid userId,ChangePasswordRequest request);

}