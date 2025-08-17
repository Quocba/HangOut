using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class PlanItemConfiguration : IEntityTypeConfiguration<PlanItem>
{
    public void Configure(EntityTypeBuilder<PlanItem> builder)
    {
        builder.HasKey(pi => pi.Id);
        
        builder.Property(pi => pi.Time)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasOne(pi => pi.Plan)
            .WithMany(p => p.PlanItems)
            .HasForeignKey(pi => pi.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(pi => pi.Business)
            .WithMany(b => b.PlanItems)
            .HasForeignKey(pi => pi.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}