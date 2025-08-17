using HangOut.Domain.Entities.Common;
using HangOut.Domain.Enums;

namespace HangOut.Domain.Entities;

public class Account : EntityAuditBase<Guid>
{
    public string? Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ERoleEnum Role { get; set; }
    public bool Active { get; set; }
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();
    public virtual ICollection<AccountVoucher> AccountVouchers { get; set; } = new List<AccountVoucher>();
    public virtual ICollection<MyFavourite> MyFavourites { get; set; } = new List<MyFavourite>();

}