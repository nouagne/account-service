using AccountService.Infrastructure.Data.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AccountService.IntegrationTests.Startup;

public class EfCoreMigrationApplier : IMigrationApplier
{
    public async Task ApplyAsync(string provider, string connectionString, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("provider is required", nameof(provider));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("connectionString is required", nameof(connectionString));

        switch (provider.ToLowerInvariant())
        {
            case "postgres":
                await ApplyPostgresAsync(connectionString, ct);
                break;

            // Exemple si tu ajoutes plus tard un provider relationnel :
            // case "sqlite": await ApplySqliteAsync(connectionString, ct); break;

            default:
                // NoSQL (mongo/dynamo/cosmos) : aucune migration EF à exécuter
                break;
        }
    }
    
    private static async Task ApplyPostgresAsync(string cs, CancellationToken ct)
    {
        // 1) Construire un ServiceProvider isolé pour ce run de migration
        var services = new ServiceCollection();

        // 2) Enregistrer AppDbContext pointant vers la DB DE TEST + bon assembly de migrations
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(cs, b =>
            {
                // IMPORTANT: pointer exactement sur l'assembly qui contient les migrations
                b.MigrationsAssembly(typeof(MigrationAssemblyMarker).Assembly.FullName);
                // (optionnel) table d’historique dans 'public'
                b.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            })
        );

        // 3) Construire le SP et créer un scope
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Logs utiles de debug
        var csb = new Npgsql.NpgsqlConnectionStringBuilder(cs);
        if (string.Equals(csb.Database, "postgres", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("[TEST] Refusing to run migrations on the 'postgres' database. Use a test DB.");
        Console.WriteLine($"[TEST] EF will migrate DB = {csb.Database} on {csb.Host}:{csb.Port}");
        Console.WriteLine($"[TEST] Migrations asm: {typeof(MigrationAssemblyMarker).Assembly.FullName}");

        var all = db.Database.GetMigrations().ToArray();
        var pendingBefore = db.Database.GetPendingMigrations().ToArray();
        Console.WriteLine($"[TEST] All migs      : {(all.Length == 0 ? "(none)" : string.Join(", ", all))}");
        Console.WriteLine($"[TEST] Pending before: {(pendingBefore.Length == 0 ? "(none)" : string.Join(", ", pendingBefore))}");

        // 4) Appliquer schéma
        if (all.Length == 0)
        {
            Console.WriteLine("[TEST] No migrations found → EnsureCreated()");
            await db.Database.EnsureCreatedAsync(ct);
        }
        else
        {
            await db.Database.MigrateAsync(ct);
        }

        // 5) Filet de sécurité pour l'extension citext (si pas déjà dans une migration)
        if (db.Database.IsNpgsql())
        {
            await db.Database.ExecuteSqlRawAsync(@"CREATE EXTENSION IF NOT EXISTS citext;", ct);
        }

        var pendingAfter = db.Database.GetPendingMigrations().ToArray();
        Console.WriteLine($"[TEST] Pending after : {(pendingAfter.Length == 0 ? "(none)" : string.Join(", ", pendingAfter))}");
    }
}