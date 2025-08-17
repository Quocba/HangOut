using HangOut.Domain.Payload.Settings;
using HangOut.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Supabase;

namespace HangOut.Domain;

public static class ConfigureServices
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<HangOutContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
                builder => builder.MigrationsAssembly(typeof(HangOutContext).Assembly.FullName));
        }); 
        services.AddScoped<HangOutContextSeed>();
        services.AddRedis(configuration);
        services.AddSupabase(configuration);
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<SupabaseSettings>(configuration.GetSection("SupabaseSettings"));
            
        return services;
    }
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (redisConnectionString != null)
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));
        return services;
    }
    public static IServiceCollection AddSupabase(this IServiceCollection services, IConfiguration configuration)
    {
        var supabaseSettings = configuration.GetSection("SupabaseSettings").Get<SupabaseSettings>();
        services.AddScoped<Supabase.Client>(_ =>
            new Supabase.Client(
                supabaseSettings.SupabaseUrl,
                supabaseSettings.SupabaseKey,
                new SupabaseOptions()
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true
                }
            ));
        return services;
    }
}