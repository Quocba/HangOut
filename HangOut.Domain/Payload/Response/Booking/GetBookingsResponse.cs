using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Booking
{
    public class GetBookingsResponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string UserName {  get; set; }
        public string Phone {  get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
