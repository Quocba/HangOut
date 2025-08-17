using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Authentication;
using HangOut.Domain.Payload.Response.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Authentication.AuthenticationEndpoint)]
public class AuthenticationController : BaseController<AuthenticationController>
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticationController(ILogger logger, IAuthenticationService authenticationService) : base(logger)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost(ApiEndPointConstant.Authentication.Login)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        var response = await _authenticationService.LoginAsync(loginRequest);
        return Ok(response);
    }
    [HttpPost(ApiEndPointConstant.Authentication.Otp)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> SendOtpRequest([FromBody] SendOtpRequest sendOtpRequest)
    {
        var response = await _authenticationService.SendOtpRequest(sendOtpRequest);
        return Ok(response);
    }
    [HttpPatch(ApiEndPointConstant.Authentication.ForgetPassword)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> ForgetPasswordAsync([FromBody] ForgotPasswordRequest forgotPasswordRequest)
    {
        var response = await _authenticationService.ForgetPasswordAsync(forgotPasswordRequest);
        return Ok(response);
    }

    [HttpPatch("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        try
        {
            var userId = UserUtil.GetAccountId(HttpContext);
            var response = await _authenticationService.ChangePassword(userId!.Value, changePasswordRequest);
            return StatusCode(response.Status, response);
        }
        catch (Exception ex)
        {
            _logger.Error("[Change Password API]" + ex.Message,ex.StackTrace);
            return StatusCode(500,ex.ToString());
        }
    }
    
}