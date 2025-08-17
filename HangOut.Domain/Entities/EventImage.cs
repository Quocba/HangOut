using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class EventImage: EntityAuditBase<Guid>
{
    public string Url { get; set; }
    public Guid EventId { get; set; }
    public virtual Event? Event { get; set; }
    
}