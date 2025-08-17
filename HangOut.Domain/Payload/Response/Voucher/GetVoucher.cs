using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Voucher
{
    public class GetVoucher
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifedDate { get; set; }

        public string BusinessImage {  get; set; }
        public string BusinessName {  get; set; }
        public string Address {  get; set; }
    }
}
