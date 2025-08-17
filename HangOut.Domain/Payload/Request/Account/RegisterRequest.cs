using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.Account;

public class RegisterRequest
{
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string? Phone { get; set; } = string.Empty;
    [Required]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 20 ký tự.")]
    public string Password { get; set; } = string.Empty;
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Tên phải có độ dài từ 1 đến 100 ký tự.")]
    public string Name { get; set; } = string.Empty;
    public IFormFile? AvatarImage { get; set; } = null!;
    [Required]
    [StringLength(maximumLength: 4, MinimumLength = 4, ErrorMessage = "Mã OTP phải có độ dài chính xác 4 ký tự.")]
    public string Otp { get; set; } = string.Empty;
}