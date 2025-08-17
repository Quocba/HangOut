using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class PlanItem : EntityBase<Guid>
{
    public TimeOnly Time { get; set; }
    
    public Guid PlanId { get; set; }
    public Guid BusinessId { get; set; }
    
    public virtual Plan Plan { get; set; }
    public virtual Business Business { get; set; }
}