using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Request.Authentication
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword {  get; set; }

        [Required(ErrorMessage = "Old password is required")]
        public string NewPassword { get; set; }
    }
}
