namespace HangOut.Domain.Payload.Response.Plan;

public class GetPlanItemsResponse
{
    public Guid Id { get; set; }
    public TimeOnly Time { get; set; }
    public GetBusinessPlanItemsResponse Business { get; set; } = new();
}
public class GetBusinessPlanItemsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Vibe { get; set; }
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MainImageUrl { get; set; }
    public string? OpeningHours { get; set; }
    public DayOfWeek? StartDay { get; set; }
    public DayOfWeek? EndDay { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public GetCategoryForPlanItemsResponse Category { get; set; } = new();
}

public class GetCategoryForPlanItemsResponse()
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
}