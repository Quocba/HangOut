using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class MyFavourite : EntityBase<Guid>
{
    public Guid AccountId { get; set; }
    public Guid BusinessId { get; set; }
    
    public virtual Account Account { get; set; }
    public virtual Business Business { get; set; }
}