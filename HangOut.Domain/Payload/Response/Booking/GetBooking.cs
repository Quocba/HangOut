using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Booking
{
    public class GetBooking
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool Active { get; set; }
        public DateTime? CancelAt { get; set; }
        public string? CancelReason { get; set; }
        public string UserName {  get; set; }
        public string UserEmail { get; set; }
        public string Phone {  get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
