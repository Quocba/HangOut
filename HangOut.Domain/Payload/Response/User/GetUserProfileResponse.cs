namespace HangOut.Domain.Payload.Response.User;

public class GetUserProfileResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } 
    public string? Phone { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? Avatar { get; set; }
}