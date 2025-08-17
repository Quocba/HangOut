using System.Linq.Expressions;
using System.Numerics;
using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Voucher;
using HangOut.Domain.Payload.Response.Voucher;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace HangOut.API.Services.Implement
{
    public class VoucherService : BaseService<VoucherService>, IVoucherService
    {
        public VoucherService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {

        }

        public async Task<ApiResponse<string>> CLickIsUsed(Guid accountId, Guid voucherId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
               Expression<Func<AccountVoucher,bool>> predicate = x => x.VoucherId == voucherId && x.AccountId == accountId;
               var getUserVoucher = await _unitOfWork.GetRepository<AccountVoucher>().SingleOrDefaultAsync(predicate: predicate, 
                                                                                                           include: i => i.Include(x => x.Voucher)
                                                                                                           .Include(x => x.Account));
                if (getUserVoucher == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Voucher not found",
                        Data = null
                    };
                }

                if (getUserVoucher.IsUsed == true)
                {
                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = $"{getUserVoucher.Account.Email} has already used this voucher",
                        Data = null
                    };
                }

                getUserVoucher.IsUsed = true;
                getUserVoucher.Voucher.Quantity -= 1;

                _unitOfWork.GetRepository<AccountVoucher>().UpdateAsync(getUserVoucher);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
              
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = $"Success. Quantity remaining {getUserVoucher.Voucher.Quantity}",
                    Data = null
                };
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<ApiResponse<string>> CreateVoucher(Guid userId,CreateVoucherRequest request)
        {
            try
            {
                var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.AccountId == userId);
                if (getBusiness == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Business Not Found",
                        Data = null
                    };
                }


                var createNewVoucher = new Voucher
                {
                    Id = Guid.NewGuid(),
                    Percent = request.Percent,
                    Name = request.VoucherName,
                    Active = true,
                    ValidFrom = request.ValidFrom,
                    ValidTo = request.ValidTo,
                    Quantity = request.Quantity,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = null,
                    BusinessId = getBusiness.Id,
                    VoucherCode = request.VoucherCode ?? UserUtil.GenerateVoucherCode()
                };

                await _unitOfWork.GetRepository<Voucher>().InsertAsync(createNewVoucher);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Create voucher success",
                    Data = null
                };
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex);
            }
        }

        public async Task<ApiResponse<string>> DeleteVoucher(Guid voucherId)
        {
            try
            {
                var checkDelete = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: x => x.Id == voucherId);
                if (checkDelete == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Voucher not found",
                        Data = null
                    };
                }

                checkDelete.Active = false;
                 _unitOfWork.GetRepository<Voucher>().UpdateAsync(checkDelete);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Delete voucher success",
                    Data = null
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString(), ex);
            }
        }

        public async Task<ApiResponse<EditVoucherRequest>> EditVoucher(EditVoucherRequest request)
        {
            try
            {
                var checkUpdate = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: x => x.Id == request.VoucherId);

                if (checkUpdate == null)
                {
                    return new ApiResponse<EditVoucherRequest>
                    {
                        Status = 404,
                        Message = "Voucher not found",
                        Data = null
                    };
                }

                checkUpdate.Percent = request.Percent != 0 ? request.Percent : checkUpdate.Percent;
                checkUpdate.Name = request.Name ?? checkUpdate.Name;
                checkUpdate.Active = checkUpdate.Active;
                checkUpdate.ValidFrom = request.ValidFrom != null ? request.ValidFrom : checkUpdate.ValidFrom;
                checkUpdate.ValidTo = request.ValidTo != null ? request.ValidTo : checkUpdate.ValidTo;
                checkUpdate.Quantity = request.Quantity != 0 ? request.Quantity : checkUpdate.Quantity;
                checkUpdate.CreatedDate = checkUpdate.CreatedDate;
                checkUpdate.LastModifiedDate = DateTime.Now;
                _unitOfWork.GetRepository<Voucher>().UpdateAsync(checkUpdate);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<EditVoucherRequest>
                {
                    Status = 200,
                    Message = "Edit Voucher Success",
                    Data = null
                };
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());

            }
        }

        public async Task<ApiResponse<Paginate<GetUserVoucherByBusiness>>> GetUserVoucherByBusiness(int pageNumber, int pageSize, 
                                                                                                    Guid userBusinessId, string? email)
        {
            var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.AccountId == userBusinessId);

            Expression<Func<AccountVoucher, bool>> predicate = x => x.Voucher.BusinessId == getBusiness.Id;

            if (!string.IsNullOrEmpty(email))
            {
                predicate = x => x.Voucher.BusinessId == getBusiness.Id && x.Account.Email.Contains(email);
            }
            var getUserVoucher = await _unitOfWork.GetRepository<AccountVoucher>()
                                                  .GetPagingListAsync(predicate: predicate ,
                                                  include: i => i.Include(x => x.Voucher).ThenInclude(x => x.Business)
                                                  .Include(a => a.Account),
                                                  page: pageNumber,
                                                  size:pageSize);

            var mapItem = getUserVoucher.Items.Select(x => new GetUserVoucherByBusiness
            {
                Id = x.VoucherId,
                Name = x.Voucher.Name,
                Percent = x.Voucher.Percent,
                ValidFrom = x.Voucher.ValidFrom,
                ValidTo = x.Voucher.ValidTo,
                AccountId = x.AccountId,
                Email = x.Account.Email,
                IsUsed = x.IsUsed,
                FullName = x.Account.Users.Select(u => u.Name).FirstOrDefault(),
                Avatar = x.Account.Users.Select(u => u.Avatar).FirstOrDefault()
            }).ToList();

            var pageResponse = new Paginate<GetUserVoucherByBusiness>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getUserVoucher.Total,
                TotalPages = (int)Math.Ceiling((double)getUserVoucher.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetUserVoucherByBusiness>>
            {
                Status = 200,
                Message = "Success",
                Data = pageResponse
            };
        }

        public async Task<ApiResponse<GetVoucher>> GetVoucher(Guid voucherId)
        {
            var getVoucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: x => x.Id == voucherId, include: i => i.Include(x => x.Business));
            if (getVoucher == null) {

                return new ApiResponse<GetVoucher>
                {
                    Status = 404,
                    Message = "Voucher not found",
                    Data = null
                };
            }

            var reponse = new GetVoucher
            {
               Id = getVoucher.Id,
               Name = getVoucher.Name,
               Percent = getVoucher.Percent,
               ValidFrom = getVoucher.ValidFrom,
               ValidTo = getVoucher.ValidTo,
               Quantity = getVoucher.Quantity,
               CreatedDate = getVoucher.CreatedDate,
               LastModifedDate = getVoucher.LastModifiedDate,
               BusinessImage = getVoucher.Business.MainImageUrl,
               BusinessName = getVoucher.Business.Name,
               Address = getVoucher.Business.Address
            };

            return new ApiResponse<GetVoucher>
            {
                Status = 200,
                Message = "Get voucher success",
                Data = reponse
            };
        }

        public async Task<ApiResponse<Paginate<GetMyVoucherResponse>>> GetVoucherByAccount(Guid accountId, int pageNumber, int pageSize)
        {
            var getVouchers = await _unitOfWork.GetRepository<AccountVoucher>().GetPagingListAsync(
                predicate: x => x.AccountId == accountId,
                include: i => i.Include(x => x.Voucher),
                page: pageNumber,
                size: pageSize
                );

            var mapItems = getVouchers.Items.Select(v => new GetMyVoucherResponse
            {
                Id = v.Voucher.Id,
                Name = v.Voucher.Name,
                Percent = v.Voucher.Percent,
                Quantity = v.Voucher.Quantity,
                ValidFrom = v.Voucher.ValidFrom,
                ValidTo = v.Voucher.ValidTo,
                Active = v.Voucher.Active,
                isUsed = v.IsUsed,
                BusinessId = v.Voucher.BusinessId,
            }).ToList();

            var pagedResponse = new Paginate<GetMyVoucherResponse>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = getVouchers.Total,
                TotalPages = (int)Math.Ceiling((double)getVouchers.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetMyVoucherResponse>>
            {
                Status = 200,
                Message = "Get my vouchers success",
                Data = pagedResponse
            };
        }

        public async Task<ApiResponse<Paginate<GetVouchersResponse>>> GetVoucherByBusiness(Guid businessId, int pageNumber, int pageSize)
        {
            Expression<Func<Voucher,bool>> expression = x => x.Active == true && x.BusinessId == businessId;

            var getVouchers = await _unitOfWork.GetRepository<Voucher>().GetPagingListAsync(predicate: expression,
                page: pageNumber,
                size: pageSize);

            var mapItems = getVouchers.Items.Select(v => new GetVouchersResponse
            {
                Id = v.Id,
                Name = v.Name,
                Percent = v.Percent,
                Quantity = v.Quantity,
                VoucherCode = v.VoucherCode,
                ValidFrom = v.ValidFrom,
                ValidTo = v.ValidTo,
                Active = v.Active
            }).ToList();

            var pagedResponse = new Paginate<GetVouchersResponse>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = getVouchers.Total,
                TotalPages = (int)Math.Ceiling((double)getVouchers.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetVouchersResponse>>
            {
                Status = 200,
                Message = "Get voucher success",
                Data  = pagedResponse
            };
            
        }

        public async Task<ApiResponse<Paginate<GetVouchersResponse>>> GetVouchersByBusinessOwner(Guid accountId, int pageNumber, int pageSize)
        {
            var getBusinessByOwner = await _unitOfWork.GetRepository<Business>().GetListAsync(predicate: x => x.AccountId == accountId);

            var listBusinessIdOfOwner = getBusinessByOwner.Select(x => x.Id).ToList();

            var getVoucherOfBusinessOwner = await _unitOfWork.GetRepository<Voucher>().GetPagingListAsync(
                predicate: x => listBusinessIdOfOwner.Contains(x.BusinessId),
                page: pageNumber,
                size: pageSize);

            var mapItem = getVoucherOfBusinessOwner.Items.Select(v => new GetVouchersResponse
            {
                Id = v.Id,
                Name = v.Name,
                Percent = v.Percent,
                Quantity = v.Quantity,
                VoucherCode = v.VoucherCode,
                ValidFrom = v.ValidFrom,
                ValidTo = v.ValidTo,
                Active = v.Active

            }).ToList();


            var pagedResponse = new Paginate<GetVouchersResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getVoucherOfBusinessOwner.Total,
                TotalPages = (int)Math.Ceiling((double)getVoucherOfBusinessOwner.Total/pageSize)
            };

            return new ApiResponse<Paginate<GetVouchersResponse>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get voucher success",
                Data = pagedResponse
            };
        }

        public async Task<ApiResponse<string>> ReceiveVoucher(Guid accountId, Guid voucherId)
        {
            try
            {
                var checkAlready = await _unitOfWork.GetRepository<AccountVoucher>().SingleOrDefaultAsync(
                    predicate: x => x.AccountId  == accountId && x.VoucherId == voucherId);

                if(checkAlready != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 208,
                        Message = "You have received this voucher in the past",
                        Data = null
                    };
                };

                var receiveNewVoucher = new AccountVoucher
                {
                    AccountId = accountId,
                    VoucherId = voucherId,
                    IsUsed = false
                };

                await _unitOfWork.GetRepository<AccountVoucher>().InsertAsync(receiveNewVoucher);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Receive voucher success",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
