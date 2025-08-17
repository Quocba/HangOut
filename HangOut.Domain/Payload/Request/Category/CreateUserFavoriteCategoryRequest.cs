namespace HangOut.Domain.Payload.Request.Category;

public class CreateUserFavoriteCategoryRequest
{
    public List<Guid> CategoryIds { get; set; } = new();
}