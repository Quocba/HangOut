using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.Review;

public class CreateReviewRequest
{
    public Guid BusinessId { get; set; }
    public string? Content { get; set; }
    public int Rating { get; set; }
    public IFormFile? Image { get; set; }
}