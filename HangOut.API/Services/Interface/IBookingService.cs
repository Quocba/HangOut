using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Response.Booking;

namespace HangOut.API.Services.Interface
{
    public interface IBookingService
    {
        Task<ApiResponse<string>> Booking(Guid accountId,Guid businessId,DateTime date);
        Task<ApiResponse<Paginate<GetBookingsResponse>>> GetBookings(int pageNumber, int pageSize);
        Task<ApiResponse<GetBooking>> GetBooking(Guid bookingId);
        Task<ApiResponse<Paginate<GetBookingsResponse>>> GetBookingByUser(Guid userId, int pageNumber, int pageSize);
    }
}
