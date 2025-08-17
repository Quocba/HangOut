using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Percent)
            .IsRequired();

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.Active)
            .IsRequired();

        builder.Property(v => v.ValidFrom)
            .IsRequired();

        builder.Property(v => v.ValidTo)
            .IsRequired();
        
        builder.Property(v => v.Quantity)
            .IsRequired();
        
        builder.HasOne(v => v.Business)
            .WithMany(b => b.Vouchers)
            .HasForeignKey(v => v.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}