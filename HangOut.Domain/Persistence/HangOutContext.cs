using System.Reflection;
using HangOut.Domain.Entities;
using HangOut.Domain.Entities.Common.Interface;
using Microsoft.EntityFrameworkCore;

namespace HangOut.Domain.Persistence;

public class HangOutContext : DbContext
{
    public HangOutContext() { }
    
    public HangOutContext(DbContextOptions<HangOutContext> options) : base(options)
    {
    }
    public DbSet<Account> Account { get; set; }
    public DbSet<AccountVoucher> AccountVoucher { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Business> Business { get; set; }
    public DbSet<Event> Event { get; set; }
    public DbSet<BusinessImage> BusinessImage { get; set; }
    public DbSet<EventImage> EventImage { get; set; }
    public DbSet<Plan> Plan { get; set; }
    public DbSet<PlanItem> PlanItem { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Voucher> Voucher { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<MyFavourite> MyFavourite { get; set; }
    public DbSet<UserFavoriteCategories> UserFavoriteCategories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified ||
                        e.State == EntityState.Added ||
                        e.State == EntityState.Deleted);

        foreach (var item in modified)
            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedEntity)
                    {
                        addedEntity.CreatedDate = DateTime.UtcNow;
                        item.State = EntityState.Added;
                    }

                    break;
                case EntityState.Modified:
                    if (item.Entity is IDateTracking modifiedEntity)
                    {
                        Entry(item.Entity).Property("Id").IsModified = false;
                        modifiedEntity.LastModifiedDate = DateTime.UtcNow;
                        item.State = EntityState.Modified;
                    }

                    break;
            }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}