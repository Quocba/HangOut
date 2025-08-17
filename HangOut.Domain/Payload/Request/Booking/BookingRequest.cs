using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Request.Booking
{
    public class BookingRequest
    {
        public Guid BusinessId { get; set; }
        public DateTime Date {  get; set; }
    }
}
