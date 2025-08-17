using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class Category : EntityAuditBase<Guid>
{
    public string Name { get; set; }
    public string? Image { get; set; }
    
    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();
    public virtual ICollection<UserFavoriteCategories> UserFavoriteCategories { get; set; } = new List<UserFavoriteCategories>();       
}