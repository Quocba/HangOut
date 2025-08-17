using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Payload.Request.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers
{
    [ApiController]
    [Route("api/v1/booking")]
    public class BookingController(ILogger _logger, IBookingService _bookingService) : Controller
    {

        [HttpPost("booking")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Booking([FromBody] BookingRequest request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _bookingService.Booking(accountId!.Value, request.BusinessId, request.Date);
                return StatusCode(response.Status,response);
            }
            catch (Exception ex) {

                _logger.Error("[Booking API] " + ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpGet("get-bookings")]
        [Authorize("BusinessOwner,Admin")]
        public async Task<IActionResult> GetBookings([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var response = await _bookingService.GetBookings(pageNumber, pageSize);
            return StatusCode(response.Status,response);
        }

        [HttpGet("get-booking")]
        public async Task<IActionResult> GetBooking([FromQuery]Guid bookingId)
        {
            var response = await _bookingService.GetBooking(bookingId);
            return StatusCode(response.Status,response);
        }

        [HttpGet("get-bookings-by-user")]
        [Authorize]
        public async Task<IActionResult> GetBookingByUser([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _bookingService.GetBookingByUser(accountId!.Value, pageNumber, pageSize);
            return StatusCode(response.Status,response);
        }
    }
}
