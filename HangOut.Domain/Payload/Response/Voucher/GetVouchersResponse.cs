using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Voucher
{
    public class GetVouchersResponse
    {
        public Guid Id { get; set; }
        public string Name {  get; set; }
        public decimal Percent {  get; set; }
        
        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }
        public string VoucherCode { get; set; }
        
        public int Quantity {  get; set; }
        public bool Active { get; set; }
        public float Rating {  get; set; }
    }
}
