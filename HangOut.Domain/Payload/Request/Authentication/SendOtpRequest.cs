using System.ComponentModel.DataAnnotations;
using HangOut.Domain.Enums;

namespace HangOut.Domain.Payload.Request.Authentication;

public class SendOtpRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
    public string Email { get; set; } = null!;
    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
    public string? Phone { get; set; } = null!;
    
    public EOtpType OtpType { get; set; }
}