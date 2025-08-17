using HangOut.API.Common.Utils;
using HangOut.API.Common.Validators;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Review;
using HangOut.Domain.Payload.Response.Review;
using Microsoft.AspNetCore.Mvc;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Review.ReviewEndpoint)]
public class ReviewController : BaseController<ReviewController>
{
    private readonly IReviewService _reviewService;
    public ReviewController(ILogger logger, IReviewService reviewService) : base(logger)
    {
        _reviewService = reviewService;
    }

    [CustomAuthorize(ERoleEnum.User)]
    [HttpPost(ApiEndPointConstant.Review.ReviewEndpoint)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReview([FromForm] CreateReviewRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        if (accountId == null || accountId == Guid.Empty)
        {
            return BadRequest("Không tìm thấy thông tin người dùng");
        }
        var response = await _reviewService.CreateReviewAsync(accountId, request);
        return CreatedAtAction(nameof(CreateReview), response);
    }
    [CustomAuthorize(ERoleEnum.BusinessOwner)]
    [HttpGet(ApiEndPointConstant.Review.ReviewEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllReviews([FromQuery] int page = 1, [FromQuery] int size = 10, 
        [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var response = await _reviewService.GetAllReviews(accountId, page, size, sortBy, isAsc);
        return Ok(response);
    }
}