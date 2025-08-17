using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class Booking : EntityAuditBase<Guid>
{
    public DateTime Date { get; set; }
    public bool Active { get; set; }
    public DateTime? CancelAt { get; set; }
    public string? CancelReason { get; set; }
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    
    public virtual User User { get; set; }
    public virtual Business Business { get; set; }
}