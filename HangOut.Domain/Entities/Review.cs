using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class Review : EntityAuditBase<Guid>
{
    public string? Content { get; set; }
    public int Rating { get; set; }
    public bool Active { get; set; }
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public string? ImageUrl { get; set; }
    public virtual User User { get; set; }
    public virtual Business Business { get; set; }
}