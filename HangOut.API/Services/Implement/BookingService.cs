using System.Numerics;
using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Entities;
using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Authentication;
using HangOut.Domain.Payload.Response.Booking;
using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace HangOut.API.Services.Implement
{
    public class BookingService : BaseService<BookingService>, IBookingService
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        public BookingService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger, IHttpContextAccessor httpContextAccessor
            , IEmailService emailService, IWebHostEnvironment env) : base(unitOfWork, logger, httpContextAccessor)
        {
            _emailService = emailService;
            _env = env;
        }

        public async Task<ApiResponse<string>>Booking(Guid accountId,Guid businessId,DateTime date)
        {
            try
            {
                var checkAlready = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                    predicate: x => x.BusinessId == businessId && x.Date == date);
                if (checkAlready != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"You booked this business on date{date}",
                        Data = null
                    };
                }

                var getUserByAccount = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId,
                    include: a => a.Include(a => a.Account));
                var getBusiness = await _unitOfWork.GetRepository<Business>().SingleOrDefaultAsync(predicate: x => x.Id == businessId,
                    include: a => a.Include(a => a.Account));
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    BusinessId = businessId,
                    Date = date,
                    UserId = getUserByAccount.Id,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    CancelAt  = null,
                    CancelReason = null
                };

                await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
                await _unitOfWork.CommitAsync();
                string template = EmailMessageUtil.GetBookingTemplate("BookingTemplate.html");
                template = template.Replace("{MainImage}", getBusiness.MainImageUrl)
                                   .Replace("{Date}", date.ToString("dd/MM/yyyy"))
                                   .Replace("{BusinessName}", getBusiness.Name)
                                   .Replace("{Address}", getBusiness.Address)
                                   .Replace("{OpeningHours}", getBusiness.OpeningHours)
                                   .Replace("{StartDay}", getBusiness.StartDay?.ToString() ?? "")
                                   .Replace("{EndDay}", getBusiness.EndDay?.ToString() ?? "");

                var emailMessage = new EmailMessage()
                {
                    ToAddress = getUserByAccount.Account.Email,
                    Body = template,
                    Subject = $"[Bạn đã đặt thành công tại [{getBusiness.Name}]"
                };

                await _emailService.SendEmailAsync(emailMessage);

                string businessTemplate = EmailMessageUtil.GetBookingTemplate("BusinessNotifyBooking.html");

                // Gán lại chuỗi sau mỗi Replace
                businessTemplate = businessTemplate
                    .Replace("{Name}", getUserByAccount.Name)
                    .Replace("{Date}", date.ToString("dd/MM/yyyy"))
                    .Replace("{Phone}", getUserByAccount.Account.Phone);

                // Gửi email
                var businessEmail = new EmailMessage
                {
                    ToAddress = getBusiness.Account.Email,
                    Body = businessTemplate,
                    Subject = $"[Đơn Mới Từ Khách Hàng] {getUserByAccount.Name}"
                };

                await _emailService.SendEmailAsync(businessEmail);

                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Booking Success",
                    Data = null
                };

            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ApiResponse<Paginate<GetBookingsResponse>>> GetBookings(int pageNumber, int pageSize)
        {
            var getBookings = await _unitOfWork.GetRepository<Booking>().GetPagingListAsync(
                include: i => i.Include(u => u.User).ThenInclude(a => a.Account),
                page: pageNumber, size: pageSize);

            var mapItem = getBookings.Items.Select(b => new GetBookingsResponse
            {
                Id = b.Id,
                Date = b.Date,
                UserName = b.User.Name,
                Phone = b.User.Account.Phone,
                CreatedDate = b.CreatedDate,
                LastModifiedDate = b.LastModifiedDate
            }).ToList();

            var pagedResponse = new Paginate<GetBookingsResponse>
            {
              Items = mapItem,
              Page = pageNumber,
              Size = pageSize,
              Total = getBookings.Total,
              TotalPages = (int)Math.Ceiling((double)getBookings.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetBookingsResponse>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get bookings success",
                Data = pagedResponse
            };
        }


        public async Task<ApiResponse<GetBooking>> GetBooking(Guid bookingId)
        {
            var getBooking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: x => x.Id == bookingId,
                include: i => i.Include(b => b.Business).Include(u => u.User).ThenInclude(a => a.Account)
                );

            if (getBooking == null)
            {
                return new ApiResponse<GetBooking>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Booking not found",
                    Data = null
                };
            }

            var mapItem = new GetBooking
            {
                Id = bookingId,
                Date = getBooking.Date,
                CreatedDate= getBooking.CreatedDate,
                Active = getBooking.Active,
                CancelAt = getBooking.CancelAt,
                CancelReason = getBooking.CancelReason,
                LastModifiedDate = getBooking.LastModifiedDate,
                UserName = getBooking.User.Name,
                UserEmail = getBooking.User.Account.Email,
                Phone = getBooking.User.Account.Phone,
                BusinessName = getBooking.Business.Name,
                BusinessAddress = getBooking.Business.Address
            };


            return new ApiResponse<GetBooking>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get booking success",
                Data = mapItem
            };

        }

        public async Task<ApiResponse<Paginate<GetBookingsResponse>>> GetBookingByUser(Guid accountId, int pageNumber, int pageSize)
        {
            var getUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId);
            var getBookings = await _unitOfWork.GetRepository<Booking>().GetPagingListAsync(
                predicate: x => x.UserId == getUser.Id,
                include: i => i.Include(u => u.User).Include(a => a.User.Account).Include(b => b.Business));

            var mapItem = getBookings.Items.Select( x => new GetBookingsResponse
            {
                Id = x.Id,
                Date = x.Date,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate,
                UserName = x.User.Name,
                Phone = x.User.Account.Phone
            }).ToList();

            var pagedResponse = new Paginate<GetBookingsResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getBookings.Total,
                TotalPages = (int)Math.Ceiling((double)getBookings.Total / pageSize)
            };

            return new ApiResponse<Paginate<GetBookingsResponse>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get booking by user success",
                Data = pagedResponse
            };
        }
    }
}
