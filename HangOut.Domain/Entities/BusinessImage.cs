using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class BusinessImage : EntityAuditBase<Guid>
{
    public string Url { get; set; }
    public Guid BusinessId { get; set; }
    public virtual Business? Business { get; set; }
}