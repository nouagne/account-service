using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure.Data.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var cnx = configuration["APP_DB_CONNECTION"]
                  ?? throw new InvalidOperationException("APP_DB_CONNECTION manquant");
        
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(
                cnx,
                npg => npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            );
        });
        
        return services;
    }
}