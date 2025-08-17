using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class AccountVoucher
{
    public Guid AccountId { get; set; }
    public Guid VoucherId { get; set; }
    public bool IsUsed { get; set; }
    
    public virtual Account Account { get; set; }
    public virtual Voucher Voucher { get; set; }
}