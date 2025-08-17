using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Event
{
    public class GetEventsResponse
    {
        public Guid Id { get; set; }
        public string Name {  get; set; }
        public string MainImage {  get; set; }
        public string Description { get; set; }
        public string ComingDay { get; set; }
    }
}
