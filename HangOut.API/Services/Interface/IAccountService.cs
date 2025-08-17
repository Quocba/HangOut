using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Account;
using HangOut.Domain.Payload.Response.Account;

namespace HangOut.API.Services.Interface;

public interface IAccountService
{
    Task<ApiResponse<RegisterResponse>> RegisterUserAsync(RegisterRequest request);
}