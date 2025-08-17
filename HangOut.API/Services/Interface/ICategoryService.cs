using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Category;
using HangOut.Domain.Payload.Response.Category;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Interface;

public interface ICategoryService
{
    Task<ApiResponse> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ApiResponse> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
    Task<ApiResponse> DeleteCategoryAsync(Guid id);

    Task<ApiResponse<IPaginate<GetCategoryResponse>>> GetCategoryAsync(int page, int size, string? sortBy, bool isAsc);
    
    Task<ApiResponse> CreateUserFavoriteCategoryAsync(Guid? accountId, CreateUserFavoriteCategoryRequest request);

}