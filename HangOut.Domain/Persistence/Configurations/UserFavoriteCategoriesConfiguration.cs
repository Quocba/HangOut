using HangOut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangOut.Domain.Persistence.Configurations;

public class UserFavoriteCategoriesConfiguration : IEntityTypeConfiguration<UserFavoriteCategories>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCategories> builder)
    {
        builder.HasKey(fc => new {CategoryId = fc.CategoryId, UserId = fc.UserId});
        
        builder.HasOne(ufc => ufc.User)
            .WithMany(u => u.UserFavoriteCategories)
            .HasForeignKey(ufc => ufc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ufc => ufc.Category)
            .WithMany(c => c.UserFavoriteCategories)
            .HasForeignKey(ufc => ufc.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}