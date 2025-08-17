using System.Linq.Expressions;
using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Business;
using HangOut.Domain.Payload.Response;
using HangOut.Domain.Payload.Response.Business;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using StackExchange.Redis.Maintenance;

namespace HangOut.API.Services.Implement
{
    public class BusinessService : BaseService<BusinessService>, IBusinessService
    {
        private readonly IUploadService _uploadService;
        private readonly IRedisService _redisService;
        public BusinessService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, 
            IHttpContextAccessor httpContextAccessor, IUploadService uploadService,
            IRedisService redisService) : base(unitOfWork, logger, httpContextAccessor)
        {
            _uploadService = uploadService;
            _redisService = redisService;
        }

        public async Task<ApiResponse<string>> EditBusiness(Guid businessId,EditBusinessRequest request)
        {
            try
            {
                var checkUpdate = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.Id == businessId);
                if (checkUpdate == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Business not found",
                        Data = null
                    };
                }
                checkUpdate.Name = request.Name ?? checkUpdate.Name;
                checkUpdate.Active = checkUpdate.Active;
                checkUpdate.Vibe = request.Vibe ?? checkUpdate.Vibe;
                checkUpdate.Latitude = request.Latidue ?? checkUpdate.Latitude;
                checkUpdate.Longitude = request.Lontidue ?? checkUpdate.Longitude;
                checkUpdate.Address = request.Address ?? checkUpdate.Address;
                checkUpdate.Province = request.Province ?? checkUpdate.Province;
                checkUpdate.Description = request.Description ?? checkUpdate.Description;
                if (request.MainImage == null)
                {
                    checkUpdate.MainImageUrl = checkUpdate.MainImageUrl;
                }
                else
                {
                    checkUpdate.MainImageUrl = await _uploadService.UploadImageAsync(request.MainImage);
                }

                checkUpdate.OpeningHours = request.OpeningHours ?? checkUpdate.OpeningHours;
                checkUpdate.StartDay = request.StartDay ?? checkUpdate.StartDay;
                checkUpdate.EndDay = request.EndDay ?? checkUpdate.EndDay;
                checkUpdate.TotalLike = checkUpdate.TotalLike;
                checkUpdate.CategoryId = request.CategoryId ?? checkUpdate.CategoryId;
                checkUpdate.CreatedDate = checkUpdate.CreatedDate;
                checkUpdate.LastModifiedDate = checkUpdate.LastModifiedDate;

                _unitOfWork.GetRepository<Business>().UpdateAsync(checkUpdate);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Edit business success",
                    Data = null
                };

            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }
        }
        public async Task<ApiResponse<string>> CreateBusinessOwner(CreateBusinessOwnerRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkPhone = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Phone.Equals(request.Phone));
                if (checkPhone != null)
                {
                    return new ApiResponse<string>
                    {
                       Status = 208,
                       Message = "Phone already exist",
                       Data = request.Phone
                    };
                }

                var checkEmail = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Email.Equals(request.Email));
                if (checkEmail != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 208,
                        Message = "Email already exist",
                        Data = request.Email
                    };
                }

                var creaNewAccount = new Account
                {
                    Id = Guid.NewGuid(),
                    Phone = request.Phone,
                    Email = request.Email,
                    Active = true,
                    Password = PasswordUtil.HashPassword(request.Password),
                    Role  = Domain.Enums.ERoleEnum.BusinessOwner,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null
                };

                await _unitOfWork.GetRepository<Account>().InsertAsync(creaNewAccount);

                var createBusiness = new Business
                {
                    Id = Guid.NewGuid(),
                    Vibe = request.Vibe,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Address = request.Address,
                    Province = request.Province,
                    Name = request.BusinessName,
                    Description = request.Description,
                    AccountId = creaNewAccount.Id,
                    Active = true,
                    MainImageUrl = request.MainImage != null
                        ? await _uploadService.UploadImageAsync(request.MainImage) 
                        : null,
                    OpeningHours = request.OpenningHours,
                    StartDay = request.StartDay,
                    EndDay = request.EndDay,
                    CategoryId = request.CategoryId,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null
                };
           
                await _unitOfWork.GetRepository<Business>().InsertAsync(createBusiness);

                if (request.Image != null)
                {
                    foreach(var image in request.Image)
                    {
                        var newImage = new BusinessImage()
                        {
                            Id= Guid.NewGuid(),
                            Url = await _uploadService.UploadImageAsync(image),
                            LastModifiedDate = null,
                            BusinessId = createBusiness.Id
                        };

                        await _unitOfWork.GetRepository<BusinessImage>().InsertAsync(newImage);
                        Console.WriteLine($"Adding Image: BusinessId={newImage.BusinessId}");
                    }
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Create business success",
                    Data  = null
                };


            }
            catch (Exception ex) { 
            
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }
        public  async Task<ApiResponse<string>> RegisterBusinessOwner(RegisterBusinessRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkPhone = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Phone.Equals(request.Phone));
                if (checkPhone != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 208,
                        Message = "Phone already exist",
                        Data = request.Phone
                    };
                }

                var checkEmail = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Email.Equals(request.Email));
                if (checkEmail != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 208,
                        Message = "Email already exist",
                        Data = request.Email
                    };
                }

                var key = "otp:" + request.Email;
                var existingOtp = await _redisService.GetStringAsync(key);

                if (string.IsNullOrEmpty(existingOtp))
                    throw new BadHttpRequestException("Không tìm thấy mã OTP");
                if (!existingOtp.Equals(request.Otp))
                    throw new BadHttpRequestException("Mã OTP không chính xác");

                var creaNewAccount = new Account
                {
                    Id = Guid.NewGuid(),
                    Phone = request.Phone,
                    Email = request.Email,
                    Active = false,
                    Password = PasswordUtil.HashPassword(request.Password),
                    Role = Domain.Enums.ERoleEnum.BusinessOwner,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null
                  
                };

                await _unitOfWork.GetRepository<Account>().InsertAsync(creaNewAccount);
                var createBusiness = new Business
                {
                    Id = Guid.NewGuid(),
                    Vibe = request.Vibe,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Address = request.Address,
                    Province = request.Province,
                    Name = request.BusinessName,
                    Description = request.Description,
                    AccountId = creaNewAccount.Id,
                    Active = false,
                    MainImageUrl = request.MainImage != null
                        ? await _uploadService.UploadImageAsync(request.MainImage)
                        : null,
                    OpeningHours = request.OpenningHours,
                    StartDay = request.StartDay,
                    EndDay = request.EndDay,
                    CategoryId = request.CategoryId,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null
                };


                await _unitOfWork.GetRepository<Business>().InsertAsync(createBusiness);

                if (request.Image != null)
                {
                    foreach (var image in request.Image)
                    {
                        var newImage = new BusinessImage()
                        {
                            Id = Guid.NewGuid(),
                            Url = await _uploadService.UploadImageAsync(image),
                            BusinessId = createBusiness.Id
                        };

                        await _unitOfWork.GetRepository<BusinessImage>().InsertAsync(newImage);
                        Console.WriteLine($"Adding Image: BusinessId={newImage.BusinessId}");
                    }

                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Create business success",
                    Data = null
                };


            }
            catch (Exception ex)
            {

                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }

        }

        public async Task<ApiResponse<Paginate<BusinessListWithHotResponse>>> GetAllBusinessResponse(
            Guid? accountId, int pageNumber, int pageSize, string? category, string? province, string? businessName)
        {
            Expression<Func<Business, bool>> predicate = x => x.Active == true;

            if (!string.IsNullOrEmpty(category))
            {
                predicate = x => x.Category.Name.Contains(category) && x.Active == true;
            }

            if (!string.IsNullOrEmpty(province))
            {
                predicate = x => x.Province.Equals(province) && x.Active == true;
            }
           if(!string.IsNullOrEmpty(province) && !string.IsNullOrEmpty(category))
            {
                predicate = r => r.Category.Name.Contains(category) && r.Province.Contains(province) && r.Active == true;
            }

            if (!string.IsNullOrEmpty(businessName))
            {
                predicate = x => x.Name.Contains(businessName) && x.Active == true;
            }


            var getBusiness = await _unitOfWork.GetRepository<Business>().GetPagingListAsync(
                 predicate: predicate,
                 include: x => x.Include(x => x.Category).Include(x => x.Events).Include(u => u.MyFavourites)
                 .Include(x => x.Reviews),
                 page: pageNumber,
                 size: pageSize
            );

            var businesses = getBusiness.Items.Select(x => new GetAllBusinessResponse
            {
                Id = x.Id,
                BusinessName = x.Name,
                MainImage = x.MainImageUrl,
                StartDay = x.StartDay,
                EndDay = x.EndDay,
                OpeningHours = x.OpeningHours,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Description = x.Description,
                Province = x.Province,
                CategoryIcon = x.Category.Image,
                CategoryName = x.Category.Name,
                TotalLike = x.TotalLike,
                EventsOfBusiness = x.Events.Where(x => x.Active == true).Select(e => new EventsOfBusinessResponse
                {
                    EventId = e.Id,
                    Name = e.Name,
                    MainImage = e.MainImageUrl
                }).ToList(),

                userFavorite = x.MyFavourites.Select(f => new UserFavoriteBusiness
                {
                    AccountId = f.AccountId,
                    BusinessId = f.BusinessId

                }).ToList(),

                Rating = x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0

            }).ToList();

            var allBusinesses = await _unitOfWork.GetRepository<Business>().GetListAsync(
                predicate: x => x.Active == true,
                include: x => x.Include(x => x.Category).Include(x => x.Events));

            var hotBusinesses = allBusinesses
                .OrderByDescending(x => x.TotalLike)
                .Take(5)
                .Select(x => new GetAllBusinessResponse
                {
                    Id = x.Id,
                    BusinessName = x.Name,
                    MainImage = x.MainImageUrl,
                    StartDay = x.StartDay,
                    EndDay = x.EndDay,
                    OpeningHours = x.OpeningHours,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Province = x.Province,
                    CategoryIcon = x.Category.Image,
                    CategoryName = x.Category.Name,
                    TotalLike = x.TotalLike,
                    EventsOfBusiness = x.Events.Where(x => x.Active == true).Select(e => new EventsOfBusinessResponse
                    {
                        EventId = e.Id,
                        Name = e.Name,
                        MainImage = e.MainImageUrl
                    }).ToList(),
                    userFavorite = x.MyFavourites.Select(f => new UserFavoriteBusiness
                    {
                        AccountId = f.AccountId,
                        BusinessId = f.BusinessId

                    }).ToList()
                }).ToList();

            if (accountId != Guid.Empty)
            {
                var getFavoriteCategory = await _unitOfWork.GetRepository<UserFavoriteCategories>().GetListAsync(
                    predicate: x => x.User.AccountId == accountId,
                    include: i => i.Include(x => x.Category));

                var favoriteCategory = getFavoriteCategory.Select(x => x.Category.Name).ToHashSet();

                businesses = businesses.OrderByDescending(x => favoriteCategory.Contains(x.CategoryName)).ToList();
            }

            var responseData = new BusinessListWithHotResponse
            {
                Businesses = businesses,
                HotBusinesses = hotBusinesses
            };

            var pagedResponse = new Paginate<BusinessListWithHotResponse>
            {
                Items = new List<BusinessListWithHotResponse> { responseData },
                Page = pageNumber,
                Size = pageSize,
                Total = getBusiness.Total,
                TotalPages = (int)Math.Ceiling((double)getBusiness.Total / pageSize)
            };

            return new ApiResponse<Paginate<BusinessListWithHotResponse>>
            {
                Status = 200,
                Message = "Get business success",
                Data = pagedResponse
            };
        }


        public async Task<ApiResponse<string>> DeleteBusiness(Guid businessId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkDelete = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.Id == businessId,
                    include: i => i.Include(x => x.Events)
                    );

                if (checkDelete == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Business not found",
                        Data = null
                    };
                }

                checkDelete.Active = false;
                foreach (var eventOfBusiness in checkDelete.Events.ToList())
                {
                    eventOfBusiness.Active = false;
                    _unitOfWork.GetRepository<Event>().UpdateAsync(eventOfBusiness);
                }

                _unitOfWork.GetRepository<Business>().UpdateAsync(checkDelete);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Delete business success",
                    Data = null
                };

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ApiResponse<Domain.Payload.Response.Business.GetBusinessDetailResponse>> GetBusinessDetail(Guid businessId)
        {
            var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
                predicate: x => x.Id == businessId,
                include: i => i.Include(x => x.Category)
                    .Include(i => i.BusinessImages!
                        .Where(x => x.BusinessId == businessId))
                    .Include(i => i.Events).Include(f => f.MyFavourites));
            if (getBusiness == null)
            {
                return new ApiResponse<GetBusinessDetailResponse>
                {
                    Status= 404,
                    Message = "Business not found",
                    Data = null
                };
            }
            var mapItem = new GetBusinessDetailResponse
            {
                BusinessId = getBusiness.Id,
                Name = getBusiness.Name,
                Active = getBusiness.Active,
                Vibe = getBusiness.Vibe,
                Latitude = getBusiness.Latitude,
                Longitude = getBusiness.Longitude,
                Address = getBusiness.Address,
                Province = getBusiness.Province,
                Description = getBusiness.Description,
                MainImageUrl = getBusiness.MainImageUrl,
                OpeningHours = getBusiness.OpeningHours,
                StartDay = getBusiness.StartDay,
                EndDay = getBusiness.EndDay,
                TotalLike = getBusiness.TotalLike,
                CategoryIcon = getBusiness.Category.Image,
                Category = getBusiness.Category.Name,
                Images = getBusiness.BusinessImages.Select(c => new Domain.Payload.Response.Image.ImagesResponse
                {
                    ImageId = c .Id,
                    Url = c.Url,
                }).ToList(),

                Events = getBusiness.Events.Select(e => new EventsResponse
                {
                    EventId = e.Id,
                    Name  = e.Name,
                    StartDate = e.StartDate,
                    DueDate = e.DueDate,
                    Active = e.Active,
                    Description = e.Description,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Location = e.Location,
                    MainImageUrl = e.MainImageUrl

                }).ToList(),

                UserFavoriteBusinesses = getBusiness.MyFavourites.Select(f => new UserFavoriteBusiness
                {
                    AccountId = f.AccountId,
                    BusinessId = f.BusinessId

                }).ToList()
            };

            return new ApiResponse<GetBusinessDetailResponse>
            {
                Status = 200,
                Message = "Get business success",
                Data = mapItem
            };
        }

        public async Task<ApiResponse<string>> ActiveBusiness(Guid businessId)
        {
            try
            {
                var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.Id == businessId && x.Active == false);
                if (getBusiness == null) {

                    return new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Business not found",
                        Data = null
                    };
                }

                getBusiness.Active = true;
                _unitOfWork.GetRepository<Business>().UpdateAsync(getBusiness);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Active business success",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<ApiResponse<GetBusinessAccount>> GetBusinessAccount(Guid businessId)
        {
            var getAccountByBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(
                predicate: x => x.Id == businessId,
                include: i => i.Include(x => x.Account)
                );

            var mapItemp = new GetBusinessAccount
            {
                Id = getAccountByBusiness.Account.Id,
                Email = getAccountByBusiness.Account.Email,
                Phone = getAccountByBusiness.Account.Phone,
                CreatedDate = getAccountByBusiness.Account.CreatedDate,
                LastModifiedDate = getAccountByBusiness.Account.LastModifiedDate
            };

            return new ApiResponse<GetBusinessAccount>
            {
                Status = 200,
                Message = "Get business account success",
                Data = mapItemp
            };
        }

        public async Task<ApiResponse<Paginate<GetAllBusinessResponse>>> GetBusinessByOwner(Guid accountId, int pageNumber, int pageSize)
        {
            var getBusiness = await _unitOfWork.GetRepository<Business>().GetPagingListAsync(
                predicate: x => x.AccountId == accountId,
                include: i => i.Include(x => x.Category).Include(x => x.Events).Include(f => f.MyFavourites),
                page: pageNumber,
                size: pageSize
            );

            var mapItem = getBusiness.Items.Select(x => new GetAllBusinessResponse
            {
                Id = x.Id,
                BusinessName = x.Name,
                MainImage = x.MainImageUrl,
                StartDay = x.StartDay,
                EndDay = x.EndDay,
                OpeningHours = x.OpeningHours,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Province = x.Province,
                CategoryName = x.Category.Name,
                TotalLike = x.TotalLike,
                EventsOfBusiness = x.Events.Where(x => x.Active == true).Select(e => new EventsOfBusinessResponse
                {
                    EventId = e.Id,
                    Name = e.Name,
                    MainImage = e.MainImageUrl
                }).ToList(),
            }).ToList();

            var pagedResponse = new Paginate<GetAllBusinessResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getBusiness.Total,
                TotalPages = (int)Math.Ceiling((double)getBusiness.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetAllBusinessResponse>>
            {
                Status = 200,
                Message = "Get business by owner success",
                Data = pagedResponse
            };
        }

        public async Task<ApiResponse<Paginate<GetAllBusinessResponse>>> GetBusinessFavorite(Guid accountId, int pageNumber, int pageSize,string? categoryName)
        {

            Expression<Func<MyFavourite, bool>> predicate = x => x.AccountId == accountId;

            if (!string.IsNullOrEmpty(categoryName))
            {
                predicate = x => x.AccountId == accountId && x.Business.Category.Name.Contains(categoryName);
            }
            var getFavoriteBusiness = await _unitOfWork.GetRepository<MyFavourite>().GetPagingListAsync(
                predicate: predicate,
                include: i => i.Include(b => b.Business).ThenInclude(c => c.Category).Include(e => e.Business.Events).Include(c => c.Business.Category),
                page: pageNumber,
                size: pageSize
                );

            var mapItem = getFavoriteBusiness.Items.Select(c => new GetAllBusinessResponse
            {
                Id = c.Business.Id,
                BusinessName = c.Business.Name,
                MainImage = c.Business.MainImageUrl,
                Address = c.Business.Address,
                Province = c.Business.Province,
                Latitude = c.Business.Latitude,
                Longitude = c.Business.Longitude,
                StartDay = c.Business.StartDay,
                EndDay = c.Business.EndDay,
                OpeningHours = c.Business.OpeningHours,
                TotalLike = c.Business.TotalLike,
                CategoryIcon = c.Business.Category.Image,
                CategoryName = c.Business.Category.Name,
                EventsOfBusiness = c.Business.Events.Where(x => x.Active == true).Select(c => new EventsOfBusinessResponse
                {
                    EventId = c.Id,
                    Name = c.Name,
                    MainImage = c.MainImageUrl,
                }).ToList(),

                userFavorite = c.Business.MyFavourites.Select(f => new UserFavoriteBusiness
                {
                    AccountId = f.AccountId,
                    BusinessId = f.BusinessId

                }).ToList()

            }).ToList();

            var pagedResponse = new Paginate<GetAllBusinessResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getFavoriteBusiness.Total,
                TotalPages = (int)Math.Ceiling((double)getFavoriteBusiness.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetAllBusinessResponse>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get favorite business success",
                Data = pagedResponse
            };
        }

        public async Task<ApiResponse<string>> FavoriteBusiness(Guid accountId,Guid businessId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkFavorite = await _unitOfWork.GetRepository<MyFavourite>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId && x.BusinessId == businessId);
                var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.Id == businessId);
                if (checkFavorite != null)
                {
                    getBusiness.TotalLike = getBusiness.TotalLike - 1;

                    _unitOfWork.GetRepository<MyFavourite>().DeleteAsync(checkFavorite);
                    _unitOfWork.GetRepository<Business>().UpdateAsync(getBusiness);
                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Successfully removed favorites business",
                        Data = null
                    };
                }

                var favoriteBusiness = new MyFavourite
                {
                    AccountId = accountId,
                    BusinessId = businessId
                };

                getBusiness.TotalLike = getBusiness.TotalLike + 1;

                await _unitOfWork.GetRepository<MyFavourite>().InsertAsync(favoriteBusiness);
                _unitOfWork.GetRepository<Business>().UpdateAsync(getBusiness);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Favorite business success",
                    Data = null
                };
            }
            catch (Exception ex) { 
            
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }

        public async Task<ApiResponse<string>> CreateBusiness(CreateReviewBusiness request)
        {
            try
            {
                var createBusiness = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Active = true,
                    Vibe = request.Vibe,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Address = request.Address,
                    Province = request.Province,
                    Description = request.Description,
                    MainImageUrl = await _uploadService.UploadImageAsync(request.MainImage),
                    OpeningHours = request.OpeningHours,
                    StartDay = request.StartDay,
                    EndDay = request.EndDay,
                    CategoryId = request.CategoryId
                };

                await _unitOfWork.GetRepository<Business>().InsertAsync(createBusiness);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tạo bussiness thành công",
                    Data = null
                };
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());
            }
        }
    }
}
