using HangOut.API.Common.Exceptions;
using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Category;
using HangOut.Domain.Payload.Response.Category;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Implement;

public class CategoryService : BaseService<CategoryService>, ICategoryService
{
    public CategoryService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<ApiResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        if (request.Image != null)
        {
            if (!EmojiUtil.IsEmoji(request.Image))
                throw new BadHttpRequestException("Hình ảnh không hợp lệ, chỉ chấp nhận emoji");
        }

        var category = new Category()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Image = request.Image,
        };
        await _unitOfWork.GetRepository<Category>().InsertAsync(category);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if(!isSuccess)
            throw new Exception("Không thể tạo danh mục, vui lòng thử lại sau");
        return new ApiResponse()
        {
            Status = StatusCodes.Status201Created,
            Message = "Tạo danh mục thành công",
        };

    }

    public async Task<ApiResponse> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
            predicate: x => x.Id.Equals(id)
        );
        if (category == null)
        {
            throw new NotFoundException("Không tìm thấy danh mục");
        }
        if (request.Image != null)
        {
            if (!EmojiUtil.IsEmoji(request.Image))
                throw new BadHttpRequestException("Hình ảnh không hợp lệ, chỉ chấp nhận emoji");
            category.Image = request.Image;
        }
        category.Name = request.Name ?? category.Name;
        
        _unitOfWork.GetRepository<Category>().UpdateAsync(category);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Không thể cập nhật danh mục, vui lòng thử lại sau");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật danh mục thành công",
        };
    }

    public async Task<ApiResponse> DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
            predicate: x => x.Id.Equals(id),
            include: x => x.Include(c => c.Businesses)
        );
        if (category == null)
        {
            throw new NotFoundException("Không tìm thấy danh mục");
        }
        if(category.Businesses.Any()) 
            throw new BadHttpRequestException("Không thể xóa danh mục, danh mục này đang được sử dụng bởi một hoặc nhiều doanh nghiệp");
        
        _unitOfWork.GetRepository<Category>().DeleteAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0; 
        if (!isSuccess)
            throw new Exception("Không thể xóa danh mục, vui lòng thử lại sau");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa danh mục thành công",
        };
    }

    public async Task<ApiResponse<IPaginate<GetCategoryResponse>>> GetCategoryAsync(int page, int size, string? sortBy, bool isAsc)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
            selector: x => new GetCategoryResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Image = x.Image
            },
            page: page,
            size: size,
            sortBy: sortBy,
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetCategoryResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách danh mục thành công",
            Data = categories
        };
    }

    public async Task<ApiResponse> CreateUserFavoriteCategoryAsync(Guid? accountId, CreateUserFavoriteCategoryRequest request)
    {
        if(accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy tài khoản người dùng");
        if (request.CategoryIds.Count > 3)
        {
            throw new BadHttpRequestException("Không thể tạo danh mục yêu thích, số lượng danh mục yêu thích tối đa là 3");
        }
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.AccountId.Equals(accountId)
        );
        if (user == null)
        {
            throw new NotFoundException("Không tìm thấy người dùng");
        }
        foreach (var categoryId in request.CategoryIds)
        {
            var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(categoryId)
            );
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với ID {categoryId}");
            }
            var userFavoriteCategory = new UserFavoriteCategories()
            {
                UserId = user.Id,
                CategoryId = category.Id
            };
            await _unitOfWork.GetRepository<UserFavoriteCategories>().InsertAsync(userFavoriteCategory);
        }
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Không thể tạo danh mục yêu thích, vui lòng thử lại sau");
        return new ApiResponse()
        {
            Status = StatusCodes.Status201Created,
            Message = "Tạo danh mục yêu thích thành công",
        };
    }
}