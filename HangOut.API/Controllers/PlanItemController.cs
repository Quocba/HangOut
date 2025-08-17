using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Plan;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.PlanItem.PlanItemEndpoint)]
public class PlanItemController : BaseController<PlanItemController>
{
    private readonly IPlanService _planService;
    public PlanItemController(ILogger logger, IPlanService planService) : base(logger)
    {
        _planService = planService;
    }
    
    [HttpDelete(ApiEndPointConstant.PlanItem.PlanItemWithId)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> RemovePlanItem([FromRoute] Guid id)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.RemovePlanItemAsync(accountId, id);
        return result;
    }
    [HttpPatch(ApiEndPointConstant.PlanItem.PlanItemWithId)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> UpdatePlanItem([FromRoute] Guid id, [FromBody] UpdatePlanItemRequest request)
    {
        var accountId = UserUtil.GetAccountId(HttpContext);
        var result = await _planService.UpdatePlanItemAsync(accountId, id, request);
        return result;
    }
}