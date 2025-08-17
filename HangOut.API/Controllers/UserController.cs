using HangOut.API.Common.Utils;
using HangOut.API.Common.Validators;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Category;
using HangOut.Domain.Payload.Request.User;
using HangOut.Domain.Payload.Response.User;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.User.UserEndpoint)]
public class UserController : BaseController<UserController>
{
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;
    public UserController(ILogger logger, IUserService userService, ICategoryService categoryService) : base(logger)
    {
        _userService = userService;
        _categoryService = categoryService;
    }
    [CustomAuthorize(ERoleEnum.User)]
    [HttpGet(ApiEndPointConstant.User.Profile)]
    [ProducesResponseType(typeof(ApiResponse<GetUserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProfileAsync()
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        
        var response = await _userService.GetUserProfileAsync(accountId);
        return Ok(response);
    }
    [CustomAuthorize(ERoleEnum.User)]
    [HttpPatch(ApiEndPointConstant.User.Profile)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserProfileAsync([FromForm] UpdateProfileRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        if (accountId == null)
        {
            return BadRequest("Không tìm thấy thông tin người dùng");
        }
        
        var response = await _userService.UpdateUserProfileAsync(accountId, request);
        return Ok(response);
    }

    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpGet(ApiEndPointConstant.User.UserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<List<GetUserProfileResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 30,
        [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var response = await _userService.GetAllUsersAsync(page, size, sortBy, isAsc);
        return Ok(response);
    } 
    
    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpGet(ApiEndPointConstant.User.UserById)]
    [ProducesResponseType(typeof(ApiResponse<GetUserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Không tìm thấy thông tin người dùng");
        }
        
        var response = await _userService.GetUserDetailsAsync(id);
        return Ok(response);
    }
    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpDelete(ApiEndPointConstant.User.UserByIdRemove)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        var response = await _userService.DeleteUserAsync(id);
        return Ok(response);
    }
    [CustomAuthorize(ERoleEnum.User)]
    [HttpPost(ApiEndPointConstant.User.UserFavoriteCategories)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserFavoriteCategory([FromBody] CreateUserFavoriteCategoryRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var response = await _categoryService.CreateUserFavoriteCategoryAsync(accountId, request);
        return CreatedAtAction(nameof(CreateUserFavoriteCategory), response);
    }
    
    
}