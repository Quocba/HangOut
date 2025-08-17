using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Request.Voucher
{
    public class CreateVoucherRequest
    {
        [Range(1, 100, ErrorMessage = "Percent can be not less than 1 and Percent can not greater than 100")]
        [Required(ErrorMessage = "Percent is required")]
        public decimal Percent {  get; set; }

        [Required(ErrorMessage = "Voucher name is required")]
        public string VoucherName {  get; set; }

        [Required(ErrorMessage = "ValidFrom is required")]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "ValidTo is required")]
        public DateTime ValidTo { get; set; }

        [Range(1, int.MaxValue,ErrorMessage = "Quantity can be not less than 1")]
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity {  get; set; }

        [Required(ErrorMessage = "Voucher code is required")]
        public string VoucherCode { get; set; } 
    }
}
