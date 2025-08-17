namespace HangOut.Domain.Payload.Response.Category;

public class GetCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Image { get; set; }
}