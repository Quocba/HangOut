using System.Security.Cryptography;
using System.Text;
using HangOut.Domain.Entities;
using HangOut.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HangOut.Domain.Persistence;

public class HangOutContextSeed
{
    private readonly HangOutContext _context;
    private readonly ILogger _logger;
    public HangOutContextSeed(HangOutContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "An error occurred while migrating the database");
            throw;
        }
    }public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "An error occurred while seeding the database");
            throw;
        }
    }
    public async Task TrySeedAsync()
    {
        if (!_context.Account.Any())
        {
            await _context.Account.AddRangeAsync(
                new Account()
                {
                    Id = Guid.Parse("01972b33-9943-77fb-8867-e1be705ec1b9"),
                    Phone = "0123456789",
                    Email = "admin@hangout.com",
                    Role = ERoleEnum.Admin,
                    CreatedDate = DateTime.UtcNow,
                    Active = true,
                    Password = HashPassword("123456"),
                }
            );
        }
        //
        // await _context.Category.AddAsync(
        //     new Category()
        //     {
        //         Id = Guid.Parse("01972d30-1c9e-7573-ba21-a0c3bd97a75c"),
        //         Name = "Food",
        //         Image = "https://example.com/images/food.jpg",
        //         CreatedDate = DateTime.UtcNow,
        //         LastModifiedDate = DateTime.UtcNow
        //     }
        // );
        // await _context.Account.AddAsync(
        //     new Account()
        //     {
        //         Id = Guid.Parse("01972d32-e150-77d1-8433-b44b8dc5cd81"),
        //         Phone = "0987654321",
        //         Email = "ta@gmail.com",
        //         Role = ERoleEnum.BusinessOwner,
        //         CreatedDate = DateTime.UtcNow,
        //         Active = true,
        //         Password = HashPassword("123456"),
        //         LastModifiedDate = DateTime.UtcNow,
        //     }
        // );
        // await _context.Business.AddAsync(
        //     new Business()
        //     {
        //         Id = Guid.Parse("01972d34-02d4-75be-b78c-48371af61813"),
        //         AccountId = Guid.Parse("01972d32-e150-77d1-8433-b44b8dc5cd81"),
        //         CreatedDate = DateTime.UtcNow,
        //         LastModifiedDate = DateTime.UtcNow,
        //         Name = "Tasty Treats",
        //         Description = "A delightful place for food lovers.",
        //         Address = "123 Food Street, Flavor Town",
        //         CategoryId = Guid.Parse("01972d30-1c9e-7573-ba21-a0c3bd97a75c"),
        //         Active = true,
        //         Latitude = "12.345678",
        //         Longitude = "98.765432",
        //         MainImageUrl = "https://example.com/images/tasty_treats.jpg",
        //         Province = "HCM",
        //         EndDay = DayOfWeek.Friday,
        //         StartDay = DayOfWeek.Monday,
        //         TotalLike = 2,
        //     }
        // );
        // await _context.Plan.AddAsync(
        //     new Plan()
        //     {
        //         Id = Guid.Parse("01972d36-1c9e-7573-ba21-a0c3bd97a75c"),
        //         Name = "Weekend Foodie Plan",
        //         Location = "HCM",
        //         CreatedDate = DateTime.UtcNow,
        //         LastModifiedDate = DateTime.UtcNow,
        //         UserId = Guid.Parse("13290747-C401-4550-B57D-BAC55B4FE2B5"),
        //         PlanItems = new List<PlanItem>()
        //         {
        //             new PlanItem()
        //             {
        //                 Id = Guid.Parse("01972d38-02d4-75be-b78c-48371af61813"),
        //                 Time = TimeOnly.FromDateTime(DateTime.UtcNow),
        //                 BusinessId = Guid.Parse("01972d34-02d4-75be-b78c-48371af61813"),
        //             },
        //             new PlanItem()
        //             {
        //                 Id = Guid.Parse("01972d3a-1c9e-7573-ba21-a0c3bd97a75c"),
        //                 Time = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(2)),
        //                 BusinessId = Guid.Parse("01972d34-02d4-75be-b78c-48371af61813"),
        //             },
        //             new PlanItem()
        //             {
        //                 Id = Guid.Parse("01972d3c-02d4-75be-b78c-48371af61813"),
        //                 Time = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(4)),
        //                 BusinessId = Guid.Parse("01972d34-02d4-75be-b78c-48371af61813"),
        //             }
        //         }
        //     }
        // );
    }
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}