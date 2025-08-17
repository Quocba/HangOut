using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Account;
using HangOut.Domain.Payload.Response.Account;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Account.AccountEndpoint)]
public class AccountController : BaseController<AccountController>
{
    private readonly IAccountService _accountService;
    
    public AccountController(ILogger logger, IAccountService accountService) : base(logger)
    {
        _accountService = accountService;
    }

    [HttpPost(ApiEndPointConstant.Account.Register)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<RegisterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        var response = await _accountService.RegisterUserAsync(request);
        return CreatedAtAction(nameof(Register), response);
    }
    
}