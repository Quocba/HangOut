using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Voucher
{
    public class GetMyVoucherResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool Active {  get; set; }
        public int Quantity { get; set; }

        public bool isUsed {  get; set; }
        public Guid BusinessId { get; set; }
    }
}
