using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.User;

public class UpdateProfileRequest
{
    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
    public string? Phone { get; set; }
    [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Tên phải có độ dài từ 1 đến 100 ký tự.")]
    public string? Name { get; set; }
    public IFormFile? Image { get; set; }
}