using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace HangOut.Repository;

public static class ConfigureServices
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<HangOutContext>, UnitOfWork<HangOutContext>>();
        return services;
    }
}