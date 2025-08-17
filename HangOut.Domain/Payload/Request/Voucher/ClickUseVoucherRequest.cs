using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Request.Voucher
{
    public class ClickUseVoucherRequest
    {
        public Guid VoucherId { get; set; }

        [JsonIgnore(Condition =JsonIgnoreCondition.Always)]
        public Guid AccountId { get; set; }
    }
}
