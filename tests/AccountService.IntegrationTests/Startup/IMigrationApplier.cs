namespace AccountService.IntegrationTests.Startup;

public interface IMigrationApplier
{
    Task ApplyAsync(string provider, string connectionString, CancellationToken ct = default);
}