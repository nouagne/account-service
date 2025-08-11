using AccountService.Application.Contracts.Common;
using AccountService.Application.Contracts.Repositories;
using AccountService.Application.Contracts.Security;
using AccountService.Infrastructure.Data.Postgres;
using AccountService.Infrastructure.Data.Postgres.Repositories;
using AccountService.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        
        // Repos
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        // Services techniques (si utilisés par le handler)
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IClock, SystemClock>();
    }
}