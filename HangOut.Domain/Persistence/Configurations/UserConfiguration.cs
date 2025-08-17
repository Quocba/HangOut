using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Active)
            .IsRequired();

        builder.HasOne(u => u.Account)
            .WithMany(a => a.Users)
            .HasForeignKey(u => u.AccountId);
    }
}