using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.Event
{
    public class CreateEventRequest
    {
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        public IFormFile MainImageUrl { get; set; } = null!;
        public List<IFormFile>? Images { get; set; }
    }
}
