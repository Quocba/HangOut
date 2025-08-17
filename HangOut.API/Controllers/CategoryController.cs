using HangOut.API.Common.Validators;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Category;
using HangOut.Domain.Payload.Response.Category;
using Microsoft.AspNetCore.Mvc;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Category.CategoryEndpoint)]
public class CategoryController : BaseController<CategoryController>
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ILogger logger, ICategoryService categoryService) : base(logger)
    {
        _categoryService = categoryService;
    }
    
    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpPost(ApiEndPointConstant.Category.CategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateCategoryAsync(request);
        return CreatedAtAction(nameof(CreateCategory), result);
    }
    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpPatch(ApiEndPointConstant.Category.CategoryWithId)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, request);
        return Ok(result);
    }
    [HttpGet(ApiEndPointConstant.Category.CategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IPaginate<GetCategoryResponse>>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int size = 30,
        [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var result = await _categoryService.GetCategoryAsync(page, size, sortBy, isAsc);
        return Ok(result);
    }
    [CustomAuthorize(ERoleEnum.Admin)]
    [HttpDelete(ApiEndPointConstant.Category.CategoryWithId)]
    [ProducesResponseType(typeof(ApiResponse<IPaginate<GetCategoryResponse>>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return Ok(result);
    }
    
}