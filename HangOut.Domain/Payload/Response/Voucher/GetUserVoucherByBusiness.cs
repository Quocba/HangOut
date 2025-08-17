using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Voucher
{
    public class GetUserVoucherByBusiness
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsUsed { get; set; }
        public Guid AccountId { get; set; }
        public string Email {  get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }

    }
}
