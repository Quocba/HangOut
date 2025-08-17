using System.ComponentModel.DataAnnotations;

namespace HangOut.Domain.Payload.Request.Authentication;

public class LoginRequest
{
    [Required]
    public string EmailOrPhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
}