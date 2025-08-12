using AccountService.IntegrationTests.Databases;
using Npgsql;

namespace AccountService.IntegrationTests.Fixtures;

public class PostgresFixture : IDbStorage
{
    private readonly string _host = Environment.GetEnvironmentVariable("TEST_PG_HOST") ?? "localhost";
    private readonly int _port = int.TryParse(Environment.GetEnvironmentVariable("TEST_PG_PORT"), out var p) ? p : 5432;
    private readonly string _user = Environment.GetEnvironmentVariable("TEST_PG_USER") ?? "account_user";
    private readonly string _pwd  = Environment.GetEnvironmentVariable("TEST_PG_PASSWORD") ?? "account_pass";
    private readonly string _db   = $"app_test_{Guid.NewGuid():N}";
    
    private string ConnectionStringAdmin =>
        $"Host={_host};Port={_port};Username={_user};Password={_pwd};Database=postgres";
    
    private string ConnectionStringTestDb =>
        $"Host={_host};Port={_port};Username={_user};Password={_pwd};Database={_db}";
    
    public async Task InitializeAsync()
    {
        // Crée une base dédiée au run
        await using var admin = new NpgsqlConnection(ConnectionStringAdmin);
        await admin.OpenAsync();
        await using var create = new NpgsqlCommand($@"CREATE DATABASE ""{_db}"" OWNER ""{_user}""", admin);
        await create.ExecuteNonQueryAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        // Nettoyage final : drop la base de test
        NpgsqlConnection.ClearAllPools(); // évite les verrous résiduels

        await using var admin = new NpgsqlConnection(ConnectionStringTestDb);
        await admin.OpenAsync();

        await using (var terminate = new NpgsqlCommand(
                         $"""
                          SELECT pg_terminate_backend(pid)
                                         FROM pg_stat_activity
                                         WHERE datname = '{_db}'
                          """, admin))
        {
            await terminate.ExecuteNonQueryAsync();
        }

        await using (var drop = new NpgsqlCommand($@"DROP DATABASE IF EXISTS ""{_db}""", admin))
        {
            await drop.ExecuteNonQueryAsync();
        }
    }

    public Task ResetAsync()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, string> GetEnvironment() => new Dictionary<string, string>
    {
        ["APP_DB_PROVIDER"]   = "postgres",
        ["APP_DB_CONNECTION"] = ConnectionStringTestDb
    };

    public string GetConnectionString()
    {
        return ConnectionStringTestDb;
    }
}


/*
public class PostgresFixture
{
    public string Host { get; } = Environment.GetEnvironmentVariable("TEST_PG_HOST") ?? "localhost";
    public int    Port { get; } = int.TryParse(Environment.GetEnvironmentVariable("TEST_PG_PORT"), out var p) ? p : 5432;

    // User applicatif (celui qu’utilise ton app / tes tests)
    public string AppUser   { get; } = Environment.GetEnvironmentVariable("TEST_PG_USER") ?? "account_user";
    public string AppPwd    { get; } = Environment.GetEnvironmentVariable("TEST_PG_PASSWORD") ?? "account_pass";

    public string BuildAdminCs() =>
        $"Host={Host};Port={Port};Username={AppUser};Password={AppPwd};Database=postgres";

    public async Task<string> CreateTestDatabaseAsync(string? prefix = null)
    {
        var dbName = $"{prefix ?? "app_test"}_{Guid.NewGuid():N}";
        await using var conn = new NpgsqlConnection(BuildAdminCs());
        await conn.OpenAsync();
        // ✅ crée la base EN NOMMANT L’OWNER = utilisateur applicatif
        await using (var cmd = new NpgsqlCommand($"""CREATE DATABASE "{dbName}" OWNER "{AppUser}";""", conn))
            await cmd.ExecuteNonQueryAsync();

        // chaîne de connexion vers la DB fraîche
        return $"Host={Host};Port={Port};Database={dbName};Username={AppUser};Password={AppPwd};Include Error Detail=true";
    }

    public async Task DropDatabaseAsync(string database)
    {
        // Fermer connexions et drop
        await using var conn = new NpgsqlConnection(BuildAdminCs());
        await conn.OpenAsync();
        var dbName = new NpgsqlConnectionStringBuilder(database).Database;
        // terminate connexions
        await using (var terminate = new NpgsqlCommand(
                         $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @db;", conn))
        {
            terminate.Parameters.AddWithValue("db", dbName!);
            await terminate.ExecuteNonQueryAsync();
        }
        await using (var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{dbName}\";", conn))
            await cmd.ExecuteNonQueryAsync();
    }
}
*/