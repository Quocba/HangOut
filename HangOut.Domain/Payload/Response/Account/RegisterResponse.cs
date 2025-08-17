using HangOut.Domain.Enums;

namespace HangOut.Domain.Payload.Response.Account;

public class RegisterResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string? Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public ERoleEnum Role { get; set; }
}