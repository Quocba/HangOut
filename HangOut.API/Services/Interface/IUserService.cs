using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.User;
using HangOut.Domain.Payload.Response.User;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Interface;

public interface IUserService
{
    Task<ApiResponse<IPaginate<GetUserProfileResponse>>> GetAllUsersAsync(int page, int size, string? sortBy, bool isAsc);
    Task<ApiResponse> DeleteUserAsync(Guid userId);
    Task<ApiResponse<GetUserProfileResponse>> GetUserProfileAsync(Guid? accountId);
    Task<ApiResponse> UpdateUserProfileAsync(Guid? accountId, UpdateProfileRequest request);
    Task<ApiResponse<GetUserProfileResponse>> GetUserDetailsAsync(Guid userId);
}