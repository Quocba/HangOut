using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class BusinessImagConfiguration : IEntityTypeConfiguration<BusinessImage>
{
    public void Configure(EntityTypeBuilder<BusinessImage> builder)
    {
        builder.HasKey(bi => bi.Id);
        
        builder.Property(bi => bi.Url)
            .IsRequired();
        
        builder.HasOne(bi => bi.Business)
            .WithMany(b => b.BusinessImages)
            .HasForeignKey(bi => bi.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}