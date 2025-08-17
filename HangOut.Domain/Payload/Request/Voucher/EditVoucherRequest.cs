using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Request.Voucher
{
    public class EditVoucherRequest
    {
        [JsonIgnore(Condition =JsonIgnoreCondition.Always)]
        public Guid VoucherId { get; set; }
        public decimal Percent {  get; set; }
        public string Name {  get; set; }
        public DateTime ValidFrom {  get; set; }
        public DateTime ValidTo { get; set; }
        public int Quantity {  get; set; }
    }
}
