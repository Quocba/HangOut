using System.ComponentModel.DataAnnotations;

namespace HangOut.Domain.Payload.Request.Authentication;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
    public string Email { get; set; }
    [Required]
    [StringLength(maximumLength: 4, MinimumLength = 4, ErrorMessage = "Mã OTP phải có độ dài 4 ký tự.")]
    public string Otp { get; set; }
    [Required]
    [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 20 ký tự.")]
    public string NewPassword { get; set; }
    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmNewPassword { get; set; }
}