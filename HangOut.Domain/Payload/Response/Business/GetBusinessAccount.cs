using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Business
{
    public class GetBusinessAccount
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
