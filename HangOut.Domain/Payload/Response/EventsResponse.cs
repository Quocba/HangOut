using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response
{
    public class EventsResponse
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Location { get; set; }
        public bool Active { get; set; }
        public string? Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
