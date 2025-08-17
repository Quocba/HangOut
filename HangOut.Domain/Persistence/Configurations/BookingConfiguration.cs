using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);


        
        builder.Property(b => b.Date)
            .IsRequired();

        builder.Property(b => b.CancelReason)
            .HasMaxLength(1000);
        
        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(b => b.Business)
            .WithMany(b => b.Bookings)
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}