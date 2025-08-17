using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Review;
using HangOut.Domain.Payload.Response.Review;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Implement;

public class ReviewService : BaseService<ReviewService>, IReviewService
{
    private readonly IUploadService _uploadService;
    public ReviewService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor,
        IUploadService uploadService) : base(unitOfWork, logger, httpContextAccessor)
    {
        _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
    }

    public async Task<ApiResponse> CreateReviewAsync(Guid? accountId, CreateReviewRequest request)
    {
        if (accountId == null || accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");

        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.AccountId == accountId
        );
        
        if (user == null)
            throw new BadHttpRequestException("Không tìm thấy người dùng");
        
        var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
            predicate: x => x.Id == request.BusinessId
        );
        
        if (business == null)
            throw new BadHttpRequestException("Không tìm thấy doanh nghiệp");

        var review = new Review()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BusinessId = business.Id,
            Active = true,
            Content = request.Content,
            Rating = request.Rating,
            ImageUrl = request.Image != null 
                ? await _uploadService.UploadImageAsync(request.Image) 
                : null,
        };
        
        await _unitOfWork.GetRepository<Review>().InsertAsync(review);

        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
            throw new BadHttpRequestException("Lỗi khi tạo đánh giá");
        return new ApiResponse()
        {
            Status = StatusCodes.Status201Created,
            Message = "Tạo đánh giá thành công",
            Data = null
        };
    }

    public async Task<ApiResponse<IPaginate<GetReviewResponse>>> GetReviewsByBusinessIdAsync(Guid businessId, 
        int page, int size, string? sortBy, bool isAsc)
    {
        var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
            predicate: x => x.Id == businessId && x.Active == true
        );
        if (business == null)
            throw new BadHttpRequestException("Không tìm thấy doanh nghiệp");

        var reviews = await _unitOfWork.GetRepository<Review>().GetPagingListAsync(
            selector: x => new GetReviewResponse() 
            {
                Id = x.Id,
                Content = x.Content,
                Rating = x.Rating,
                CreatedDate = x.CreatedDate,
                BusinessId = x.BusinessId,
                LastModifiedDate = x.LastModifiedDate,
                ImageUrl = x.ImageUrl,
                User = new GetUserForReviewResponse()
                {
                    Id = x.User.Id,
                    Avatar = x.User.Avatar,
                    Name = x.User.Name
                }
            },
            predicate: x => x.BusinessId == businessId  && 
                            x.Active == true && 
                            x.User.Active == true,
            page: page,
            size: size,
            sortBy: sortBy,
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetReviewResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách đánh giá thành công",
            Data = reviews
        };
    }

    public async Task<ApiResponse<IPaginate<GetReviewResponse>>> GetAllReviews(Guid? accountId, int page, int size, string? sortBy, bool isAsc)
    {
        if(accountId == null || accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");

        var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
            predicate: x => x.AccountId == accountId && x.Active == true
        );
        if (business == null)
            throw new BadHttpRequestException("Không tìm thấy doanh nghiệp");

        var reviews = await _unitOfWork.GetRepository<Review>().GetPagingListAsync(
            selector: x => new GetReviewResponse()
            {
                Id = x.Id,
                Content = x.Content,
                Rating = x.Rating,
                CreatedDate = x.CreatedDate,
                ImageUrl = x.ImageUrl,
                BusinessId = x.BusinessId,
                LastModifiedDate = x.LastModifiedDate,
                User = new GetUserForReviewResponse()
                {
                    Id = x.User.Id,
                    Avatar = x.User.Avatar,
                    Name = x.User.Name
                }
            },
            predicate: x => x.Business.AccountId == accountId && 
                            x.Active == true && 
                            x.User.Active == true,
            page: page,
            size: size,
            sortBy: sortBy,
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetReviewResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách đánh giá thành công",
            Data = reviews
        };
    }
}