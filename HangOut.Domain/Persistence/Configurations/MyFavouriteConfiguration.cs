using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class MyFavouriteConfiguration : IEntityTypeConfiguration<MyFavourite>
{
    public void Configure(EntityTypeBuilder<MyFavourite> builder)
    {
        builder.HasKey(mf => mf.Id);
        
        builder.HasOne(mf => mf.Account)
            .WithMany(a => a.MyFavourites)
            .HasForeignKey(mf => mf.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(mf => mf.Business)
            .WithMany(b => b.MyFavourites)
            .HasForeignKey(mf => mf.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}