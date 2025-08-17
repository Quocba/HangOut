using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Review;
using HangOut.Domain.Payload.Response.Review;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Interface;

public interface IReviewService
{
    Task<ApiResponse> CreateReviewAsync(Guid? accountId, CreateReviewRequest request);
    Task<ApiResponse<IPaginate<GetReviewResponse>>> GetReviewsByBusinessIdAsync(Guid businessId, 
        int page, int size, string? sortBy, bool isAsc);
    Task<ApiResponse<IPaginate<GetReviewResponse>>> GetAllReviews(Guid? accountId, int page, int size, string? sortBy, bool isAsc);
}