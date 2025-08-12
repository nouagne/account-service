using System.Linq;
using AccountService.Infrastructure.Data.Postgres; // AppDbContext + MigrationAssemblyMarker
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AccountService.IntegrationTests;

public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    private readonly string _cs;
    public TestApiFactory(string connectionString) => _cs = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // 1) Supprime l’enregistrement "prod" du DbContext
            var toRemove = services.Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)).ToList();
            foreach (var d in toRemove) services.Remove(d);

            // 2) Réenregistre AppDbContext en pointant la DB DE TEST (_cs) + assembly des migrations
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(_cs, b =>
                {
                    b.MigrationsAssembly(typeof(MigrationAssemblyMarker).Assembly.FullName);
                    b.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                })
            );

            // 3) Applique les migrations sur la DB de test (et log)
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var csb = new NpgsqlConnectionStringBuilder(_cs);
            Console.WriteLine($"[TEST] EF will migrate DB = {csb.Database} on {csb.Host}:{csb.Port}");
            Console.WriteLine($"[TEST] Migrations asm: {typeof(MigrationAssemblyMarker).Assembly.FullName}");
            Console.WriteLine($"[TEST] All migs      : {string.Join(", ", db.Database.GetMigrations())}");
            Console.WriteLine($"[TEST] Pending before: {string.Join(", ", db.Database.GetPendingMigrations())}");

            if (!db.Database.GetMigrations().Any())
            {
                Console.WriteLine("[TEST] No migrations found → EnsureCreated()");
                db.Database.EnsureCreated();
            }
            else
            {
                db.Database.Migrate();
            }

            Console.WriteLine($"[TEST] Pending after : {string.Join(", ", db.Database.GetPendingMigrations())}");
        });
    }
}