using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangOut.Domain.Enums;

namespace HangOut.Domain.Payload.Response.Image
{
    public class ImagesResponse
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; }
    }
}
