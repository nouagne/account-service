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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Repos
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        // Services techniques (si utilisés par le handler)
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IClock, SystemClock>();
        
        var provider = configuration["APP_DB_PROVIDER"]?.ToLowerInvariant();

        return provider switch
        {
            "postgres" => services.AddPostgres(configuration), // EF Core + Npgsql
            //"sqlite"   => services.AddSqlite(config),   // EF Core + SQLite
            //"mongo"    => services.AddMongo(config),    // MongoDB.Driver
            //"dynamo"   => services.AddDynamo(config),   // AWSSDK.DynamoDBv2
            //"cosmos"   => services.AddCosmos(config),   // Microsoft.Azure.Cosmos ou EF Core Cosmos
            _ => throw new InvalidOperationException("APP_DB_PROVIDER non configuré")
        };
    }
}