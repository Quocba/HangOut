using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class EventImageConfiguration : IEntityTypeConfiguration<EventImage>
{
    public void Configure(EntityTypeBuilder<EventImage> builder)
    {
        builder.HasKey(ei => ei.Id);
        
        builder.Property(ei => ei.Url)
            .IsRequired();
        
        builder.HasOne(ei => ei.Event)
            .WithMany(e => e.EventImages)
            .HasForeignKey(ei => ei.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}