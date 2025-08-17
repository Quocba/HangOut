using HangOut.Domain.Entities;
using HangOut.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Phone)
            .HasMaxLength(20);
        
        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Password)
            .IsRequired();
        
        builder.Property(a => a.Role)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ERoleEnum)Enum.Parse(typeof(ERoleEnum), v)
            );

        builder.Property(a => a.Active)
            .IsRequired();
    }
}