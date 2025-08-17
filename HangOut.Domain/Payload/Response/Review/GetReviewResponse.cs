namespace HangOut.Domain.Payload.Response.Review;

public class GetReviewResponse
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public int Rating { get; set; }
    public Guid BusinessId { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public GetUserForReviewResponse User { get; set; } = new();
}

public class GetUserForReviewResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Avatar { get; set; }
}