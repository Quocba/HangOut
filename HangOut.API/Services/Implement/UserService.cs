using HangOut.API.Common.Exceptions;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.User;
using HangOut.Domain.Payload.Response.User;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Implement;

public class UserService : BaseService<UserService>, IUserService
{
    private readonly IUploadService _uploadService;
    public UserService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, 
        IHttpContextAccessor httpContextAccessor, IUploadService uploadService) : base(unitOfWork, logger, httpContextAccessor)
    {
        _uploadService = uploadService;
    }

    public async Task<ApiResponse<IPaginate<GetUserProfileResponse>>> GetAllUsersAsync(int page, int size, string? sortBy, bool isAsc)
    {
        var users = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
            selector: x => new GetUserProfileResponse
            {
                UserId = x.Id,
                Name = x.Name,
                Phone = x.Account.Phone,
                Email = x.Account.Email,
                Avatar = x.Avatar,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate
            },
            predicate: x => x.Active == true && x.Account.Active == true,
            include: x => x.Include(x => x.Account),
            page: page,
            size: size,
            sortBy: sortBy,
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetUserProfileResponse>>()
        {
            Status = 200,
            Message = "Lấy danh sách người dùng thành công",
            Data = users
        };
    }

    public async Task<ApiResponse> DeleteUserAsync(Guid userId)
    {
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Id.Equals(userId) && x.Active == true && x.Account.Active == true,
            include: x => x.Include(x => x.Account)
        );
        if (user == null)
            throw new NotFoundException("Không tìm thấy thông tin người dùng");
        user.Active = false;
        user.Account.Active = false;
        _unitOfWork.GetRepository<User>().UpdateAsync(user);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new BadHttpRequestException("Xóa người dùng thất bại");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa người dùng thành công"
        };
    }

    public async Task<ApiResponse<GetUserProfileResponse>> GetUserProfileAsync(Guid? accountId)
    {
        if (accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.AccountId.Equals(accountId) && x.Active == true && x.Account.Active == true,
            include: x => x.Include(x => x.Account)
        );
        if (user == null) 
            throw new NotFoundException("Không tìm thấy thông tin người dùng");

        var response = new GetUserProfileResponse()
        {
            UserId = user.Id,
            Name = user.Name,
            Phone = user.Account.Phone,
            Email = user.Account.Email,
            Avatar = user.Avatar,
            CreatedDate = user.CreatedDate,
            LastModifiedDate = user.LastModifiedDate
        };
        return new ApiResponse<GetUserProfileResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin người dùng thành công",
            Data = response
        };
    }

    public async Task<ApiResponse> UpdateUserProfileAsync(Guid? accountId, UpdateProfileRequest request)
    {
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.AccountId.Equals(accountId) && x.Active == true && x.Account.Active == true,
            include: x => x.Include(x => x.Account)
        );
        if (user == null)
            throw new NotFoundException("Không tìm thấy thông tin người dùng");
        user.Name = request.Name ?? user.Name;
        user.Account.Phone = request.Phone ?? user.Account.Phone;
        
        if(request.Image != null)
        {
            var imageUrl = await _uploadService.UploadImageAsync(request.Image);
            if (string.IsNullOrEmpty(imageUrl))
                throw new BadHttpRequestException("Không thể tải lên ảnh đại diện");
            if (!string.IsNullOrEmpty(user.Avatar) && user.Avatar != imageUrl)
            {
                var isDeleteSuccess = await _uploadService.DeleteImageAsync(user.Avatar);
                if (!isDeleteSuccess)
                    throw new BadHttpRequestException("Không thể xóa ảnh đại diện cũ");
            }
            user.Avatar = imageUrl;
        }

        _unitOfWork.GetRepository<User>().UpdateAsync(user);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new BadHttpRequestException("Cập nhật thông tin người dùng thất bại");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật thông tin người dùng thành công"
        };
    }

    public async Task<ApiResponse<GetUserProfileResponse>> GetUserDetailsAsync(Guid userId)
    {
        
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            selector: x => new GetUserProfileResponse
            {
                UserId = x.Id,
                Name = x.Name,
                Phone = x.Account.Phone,
                Email = x.Account.Email,
                Avatar = x.Avatar,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate
            },
            predicate: x => x.Id.Equals(userId) && x.Active == true && x.Account.Active == true,
            include: x => x.Include(x => x.Account)
        );
        if (user == null)
            throw new NotFoundException("Không tìm thấy thông tin người dùng");
        return new ApiResponse<GetUserProfileResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin người dùng thành công",
            Data = user
        };
    }
}