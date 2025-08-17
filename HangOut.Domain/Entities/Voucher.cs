using HangOut.Domain.Entities.Common;

namespace HangOut.Domain.Entities;

public class Voucher : EntityAuditBase<Guid>
{
    public decimal Percent { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int Quantity { get; set; }
    public Guid BusinessId { get; set; }
    public string VoucherCode { get; set; }
    public virtual Business Business { get; set; }
    public virtual ICollection<AccountVoucher> AccountVouchers { get; set; } = new List<AccountVoucher>();
}