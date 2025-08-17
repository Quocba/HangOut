using HangOut.API.Common.Exceptions;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Plan;
using HangOut.Domain.Payload.Response.Plan;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Implement;

public class PlanService : BaseService<PlanService>, IPlanService
{
    public PlanService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }
    
    public async Task<ApiResponse> CreatePlanAsync(Guid? accountId, CreatePlanRequest request)
    {
        if (accountId == null)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if(user == null)
            throw new BadHttpRequestException("Người dùng không tồn tại");

        var plan = new Plan()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Active = true,
            UserId = user.Id,
            Location = request.Location,
        };
        if (request.PlanItems != null)
        {
            foreach (var plans in request.PlanItems)
            {
                var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
                    predicate: x => x.Id == plans.BusinessId
                                    // && x.Province == request.Location
                );
                if (business == null)
                    throw new BadHttpRequestException($"Không tìm thấy doanh nghiệp với ID {plans.BusinessId}");
                var planItem = new PlanItem()
                {
                    Id = Guid.NewGuid(),
                    Time = plans.Time,
                    BusinessId = plans.BusinessId,
                    PlanId = plan.Id
                };
                plan.PlanItems.Add(planItem);
            }
        }

        await _unitOfWork.GetRepository<Plan>().InsertAsync(plan);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Tạo kế hoạch không thành công");
        return new ApiResponse
        {
            Status = StatusCodes.Status201Created,
            Message = "Tạo kế hoạch thành công"
        };
    }

    public async Task<ApiResponse> CreatePlanItemForPlanAsync(Guid? accountId, Guid planId, CreatePlanItemRequest request)
    {
        if(accountId == null)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if(user == null)
            throw new NotFoundException("Người dùng không tồn tại");

        var plan = await _unitOfWork.GetRepository<Plan>().SingleOrDefaultAsync(
            predicate: x => x.Id == planId && x.UserId == user.Id,
            include: x => x.Include(p => p.PlanItems)
        );
        if (plan == null)
            throw new NotFoundException("Kế hoạch không tồn tại");
        if (plan.PlanItems.Any(x => x.Time == request.Time))
        {
            throw new BadHttpRequestException("Kế hoạch đã có mục tại thời gian này");
        }
        if (plan == null)
            throw new NotFoundException("Kế hoạch không tồn tại");
        var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
            predicate: x => x.Id == request.BusinessId
                            // && x.Province == plan.Location
        );
        if (business == null)
            throw new NotFoundException($"Không tìm thấy doanh nghiệp với ID {request.BusinessId}");
        var planItem = new PlanItem()
        {
            Id = Guid.NewGuid(),
            Time = request.Time,
            BusinessId = request.BusinessId,
            PlanId = plan.Id
        };
        await _unitOfWork.GetRepository<PlanItem>().InsertAsync(planItem);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Tạo mục kế hoạch không thành công");
        return new ApiResponse()
        {
            Status = StatusCodes.Status201Created,
            Message = "Tạo mục kế hoạch thành công"
        };
    }

    public async Task<ApiResponse<IPaginate<GetPlansResponse>>> GetPlansForUserAsync(Guid? accountId, int page, int size, string? sortBy, bool isAsc)
    {
        if(accountId == Guid.Empty) 
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");
        var plans = await _unitOfWork.GetRepository<Plan>().GetPagingListAsync(
            selector: x => new GetPlansResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate,
                PlanItems = x.PlanItems.Select(pi => new GetPlanItemsForPlanResponse()
                {
                    Id = pi.Id,
                    Time = pi.Time,
                    Business = new GetBusinessPlanItemsForPlanResponse()
                    {
                        Id = pi.Business.Id,
                        Name = pi.Business.Name,
                        Vibe = pi.Business.Vibe,
                        Latitude = pi.Business.Latitude,
                        Longitude = pi.Business.Longitude,
                        Address = pi.Business.Address,
                        Province = pi.Business.Province,
                        Description = pi.Business.Description,
                        MainImageUrl = pi.Business.MainImageUrl,
                        OpeningHours = pi.Business.OpeningHours,
                        StartDay = pi.Business.StartDay,
                        EndDay = pi.Business.EndDay,
                        CreatedDate = pi.Business.CreatedDate,
                        LastModifiedDate = pi.Business.LastModifiedDate,
                        Category = new GetCategoryForPlanResponse()
                        {
                            Id = pi.Business.Category.Id,
                            Name = pi.Business.Category.Name,
                            Image = pi.Business.Category.Image
                        }
                    } 
                }).OrderBy(x => x.Time).ToList()
            },
            predicate: x => x.UserId == user.Id,
            page: page,
            size: size,
            sortBy: sortBy ?? nameof(Plan.CreatedDate),
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetPlansResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách kế hoạch thành công",
            Data = plans
        };
    }

    public async Task<ApiResponse<IPaginate<GetPlanItemsResponse>>> GetPlanItemsForPlanAsync(Guid? accountId, Guid planId, int page, int size, string? sortBy, bool isAsc)
    {
        if(accountId == null) 
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");

        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");
        var planItems = await _unitOfWork.GetRepository<PlanItem>().GetPagingListAsync(
            selector: x => new GetPlanItemsResponse()
            {
                Id = x.Id,
                Time = x.Time,
                Business = new GetBusinessPlanItemsResponse()
                {
                    Id = x.Business.Id,
                    Name = x.Business.Name,
                    Vibe = x.Business.Vibe,
                    Latitude = x.Business.Latitude,
                    Longitude = x.Business.Longitude,
                    Address = x.Business.Address,
                    Province = x.Business.Province,
                    Description = x.Business.Description,
                    MainImageUrl = x.Business.MainImageUrl,
                    OpeningHours = x.Business.OpeningHours,
                    StartDay = x.Business.StartDay,
                    EndDay = x.Business.EndDay,
                    CreatedDate = x.Business.CreatedDate,
                    LastModifiedDate = x.Business.LastModifiedDate,
                    Category = new GetCategoryForPlanItemsResponse()
                    {
                        Id = x.Business.Category.Id,
                        Name = x.Business.Category.Name,
                        Image = x.Business.Category.Image
                    }
                }
            },
            predicate: x => x.PlanId == planId && x.Plan.UserId == user.Id,
            page: page,
            size: size,
            sortBy: sortBy ?? "Time",
            isAsc: isAsc
        );
        return new ApiResponse<IPaginate<GetPlanItemsResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách mục kế hoạch thành công",
            Data = planItems
        };
    }

    public async Task<ApiResponse> RemovePlanItemAsync(Guid? accountId, Guid planItemId)
    {
        if(accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");
        var planItem = await _unitOfWork.GetRepository<PlanItem>().SingleOrDefaultAsync(
            predicate: x => x.Id == planItemId && x.Plan.UserId == user.Id
        );
        if (planItem == null)
            throw new NotFoundException("Mục kế hoạch không tồn tại");
        _unitOfWork.GetRepository<PlanItem>().DeleteAsync(planItem);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Xoá mục kế hoạch không thành công");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xoá mục kế hoạch thành công"
        };
    }

    public async Task<ApiResponse> RemovePlanAsync(Guid? accountId, Guid planId)
    {
        if (accountId == null)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");
        var plan = await _unitOfWork.GetRepository<Plan>().SingleOrDefaultAsync(
            predicate: x => x.Id == planId && x.UserId == user.Id,
            include: x => x.Include(p => p.PlanItems)
        );
        if (plan == null)
            throw new NotFoundException("Kế hoạch không tồn tại");
        if (plan.PlanItems.Any())
        {
            _unitOfWork.GetRepository<PlanItem>().DeleteRangeAsync(plan.PlanItems);
        }
        _unitOfWork.GetRepository<Plan>().DeleteAsync(plan);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Xoá kế hoạch không thành công");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xoá kế hoạch thành công"
        };
    }

    public async Task<ApiResponse> UpdatePlanAsync(Guid? accountId, Guid planId, UpdatePlanRequest request)
    {
        if(accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");
        var plan = await _unitOfWork.GetRepository<Plan>().SingleOrDefaultAsync(
            predicate: x => x.Id == planId && x.UserId == user.Id
        );
        if (plan == null)
            throw new NotFoundException("Kế hoạch không tồn tại");
        plan.Name = request.Name ?? plan.Name;
        plan.Location = request.Location ?? plan.Location;
        _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Cập nhật kế hoạch không thành công");
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật kế hoạch thành công"
        };
    }

    public async Task<ApiResponse> UpdatePlanItemAsync(Guid? accountId, Guid planItemId, UpdatePlanItemRequest request)
    {
        if(accountId == Guid.Empty)
            throw new BadHttpRequestException("Không tìm thấy thông tin người dùng");

        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Account.Id == accountId,
            include: x => x.Include(u => u.Account)
        );
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");

        var planItem = await _unitOfWork.GetRepository<PlanItem>().SingleOrDefaultAsync(
            predicate: x => x.Id == planItemId && x.Plan.UserId == user.Id,
            include: x => x.Include(p => p.Plan)
        );
        if (planItem == null)
            throw new NotFoundException("Mục kế hoạch không tồn tại");
        if (request.Time != null)
        {
            if (planItem.Plan.PlanItems.Any(x => x.Time == request.Time))
            {
                throw new BadHttpRequestException("Kế hoạch đã có mục tại thời gian này");
            }
            planItem.Time = request.Time.Value;
        }
        if (request.BusinessId != null)
        {
            var business = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.BusinessId 
                                // && x.Province == planItem.Plan.Location
            );
            if (business == null)
                throw new NotFoundException($"Không tìm thấy doanh nghiệp với ID {request.BusinessId}");
            planItem.BusinessId = request.BusinessId.Value;
        }
        _unitOfWork.GetRepository<PlanItem>().UpdateAsync(planItem);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
            throw new Exception("Cập nhật mục kế hoạch không thành công");
        
        return new ApiResponse()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật mục kế hoạch thành công"
        };
    }
}