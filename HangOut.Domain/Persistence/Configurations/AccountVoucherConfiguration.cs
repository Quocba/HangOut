using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class AccountVoucherConfiguration : IEntityTypeConfiguration<AccountVoucher>
{
    public void Configure(EntityTypeBuilder<AccountVoucher> builder)
    {
        builder.HasKey(v => new {AccountId = v.AccountId, VoucherId = v.VoucherId});

        builder.Property(av => av.IsUsed)
            .IsRequired();

        builder.HasOne(av => av.Account)
            .WithMany(a => a.AccountVouchers)
            .HasForeignKey(av => av.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(av => av.Voucher)
            .WithMany(v => v.AccountVouchers)
            .HasForeignKey(av => av.VoucherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}