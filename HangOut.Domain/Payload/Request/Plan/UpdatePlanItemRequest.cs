namespace HangOut.Domain.Payload.Request.Plan;

public class UpdatePlanItemRequest
{
    public TimeOnly? Time { get; set; }
    public Guid? BusinessId { get; set; }
}