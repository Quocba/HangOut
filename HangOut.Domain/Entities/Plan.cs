using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class Plan : EntityAuditBase<Guid>
{
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }
    public virtual ICollection<PlanItem> PlanItems { get; set; } = new List<PlanItem>();
    
}