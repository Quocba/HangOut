using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class UserFavoriteCategories
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }

    public virtual User User { get; set; }
    public virtual Category Category { get; set; }
}