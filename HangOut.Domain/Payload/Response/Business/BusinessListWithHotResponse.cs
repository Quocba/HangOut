using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Business
{
    public class BusinessListWithHotResponse
    {
        public List<GetAllBusinessResponse> Businesses { get; set; }
        public List<GetAllBusinessResponse> HotBusinesses { get; set; }
    }
}
