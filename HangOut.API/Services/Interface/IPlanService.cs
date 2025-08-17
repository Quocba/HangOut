using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Plan;
using HangOut.Domain.Payload.Response.Plan;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Interface;

public interface IPlanService
{
    Task<ApiResponse> CreatePlanAsync(Guid? accountId, CreatePlanRequest request);
    Task<ApiResponse> CreatePlanItemForPlanAsync(Guid? accountId, Guid planId, CreatePlanItemRequest request);
    
    Task<ApiResponse<IPaginate<GetPlansResponse>>> GetPlansForUserAsync(Guid? accountId, int page, int size, string? sortBy, bool isAsc);
    Task<ApiResponse<IPaginate<GetPlanItemsResponse>>> GetPlanItemsForPlanAsync(Guid? accountId, Guid planId, int page, int size, string? sortBy, bool isAsc);
    
    Task<ApiResponse> RemovePlanItemAsync(Guid? accountId, Guid planItemId);
    Task<ApiResponse> RemovePlanAsync(Guid? accountId, Guid planId);
    Task<ApiResponse> UpdatePlanAsync(Guid? accountId, Guid planId, UpdatePlanRequest request);
    Task<ApiResponse> UpdatePlanItemAsync(Guid? accountId, Guid planItemId, UpdatePlanItemRequest request);
}