using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class User : EntityAuditBase<Guid>
{
    public string Name { get; set; }
    public string? Avatar { get; set; }
    public bool Active { get; set; }
    public Guid AccountId { get; set; }
    
    public virtual Account Account { get; set; }
    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<UserFavoriteCategories> UserFavoriteCategories { get; set; } = new List<UserFavoriteCategories>();
}