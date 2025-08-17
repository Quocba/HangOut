using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HangOut.Domain.Enums;
using HangOut.Domain.Payload.Request.Account;
using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.Business
{
    public class CreateBusinessOwnerRequest
    {
        [Required]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 20 ký tự.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        //[Required]
        //[StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Tên phải có độ dài từ 1 đến 100 ký tự.")]
        //public string Name { get; set; } = string.Empty;
        //public IFormFile? AvatarImage { get; set; } = null!;

        [Required(ErrorMessage = "Name is required")]
        public string BusinessName { get; set; }
        public string? Vibe { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Latitude is required")]
        public string Latitude { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Longitude is required")]
        public string Longitude { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public string Province { get; set; }

        public string? Description { get; set; }

        public IFormFile? MainImage { get; set; }

        [Required(ErrorMessage = "OpenningHours is required")]
        public string OpenningHours { get; set; }

        public DayOfWeek? StartDay {  get; set; }
        public DayOfWeek? EndDay {  get; set; }
        public List<IFormFile>? Image { get; set; }
        public Guid CategoryId { get; set; }
    }
}
