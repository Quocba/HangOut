using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Plan;
using HangOut.Domain.Payload.Response.Plan;
using Microsoft.AspNetCore.Mvc;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Controllers;
[ApiController]
[Route(ApiEndPointConstant.Plan.PlanEndpoint)]
public class PlanController : BaseController<PlanController>
{
    private readonly IPlanService _planService;

    public PlanController(ILogger logger, IPlanService planService) : base(logger)
    {
        _planService = planService;
    }
    [HttpPost(ApiEndPointConstant.Plan.PlanEndpoint)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    public async Task<ApiResponse> CreatePlan([FromBody] CreatePlanRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.CreatePlanAsync(accountId, request);
        return result;
    }
    [HttpPost(ApiEndPointConstant.Plan.PlanItemsByPlanId)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    public async Task<ApiResponse> CreatePlanItemForPlan([FromRoute] Guid id, [FromBody] CreatePlanItemRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.CreatePlanItemForPlanAsync(accountId, id, request);
        return result;
    }
    [HttpGet(ApiEndPointConstant.Plan.PlanEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IPaginate<GetPlansResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans([FromQuery] int page = 1, [FromQuery] int pageSize = 30,
        [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.GetPlansForUserAsync(accountId, page, pageSize, sortBy, isAsc);
        return Ok(result);
    }
    [HttpGet(ApiEndPointConstant.Plan.PlanItemsByPlanId)]
    [ProducesResponseType(typeof(ApiResponse<IPaginate<GetPlanItemsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanItems([FromRoute] Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 30,
        [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.GetPlanItemsForPlanAsync(accountId, id, page, pageSize, sortBy, isAsc);
        return Ok(result);
    }
    [HttpDelete(ApiEndPointConstant.Plan.PlanWithId)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> RemovePlan([FromRoute] Guid id)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.RemovePlanAsync(accountId, id);
        return result;
    }
    [HttpPatch(ApiEndPointConstant.Plan.PlanWithId)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> UpdatePlan([FromRoute] Guid id, [FromBody] UpdatePlanRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.UpdatePlanAsync(accountId, id, request);
        return result;
    }
}