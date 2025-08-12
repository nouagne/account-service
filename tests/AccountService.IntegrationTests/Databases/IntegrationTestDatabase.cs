using AccountService.IntegrationTests.Startup;

namespace AccountService.IntegrationTests.Databases;

public class IntegrationTestDatabase : IAsyncLifetime
{
    private IDbStorage _dbStorage = null!;
    private IMigrationApplier _migrator = null!;
    private TestApiFactory _factory = null!;

    protected HttpClient Client { get; private set; } = null!;

    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        _dbStorage = DbStorageFactory.Create();
        await _dbStorage.InitializeAsync();
        _connectionString = _dbStorage.GetConnectionString();
        
        // pousse les variables d'env pour l'API
        foreach (var kv in _dbStorage.GetEnvironment())
            Environment.SetEnvironmentVariable(kv.Key, kv.Value);

        // 2) Migrations (EF uniquement ici)
        _migrator = new EfCoreMigrationApplier();
        await _migrator.ApplyAsync("postgres", _connectionString);
        
        // 3) Démarrer l’API
        _factory = new TestApiFactory();     // WebApplicationFactory<Program> SANS EF ni migrations
        Client  = _factory.CreateClient();
        //await _dbStorage.ResetAsync();
    }

    public async Task DisposeAsync()
    {
        // Client.Dispose();
        // await _factory.DisposeAsync();
        // await _dbStorage.DisposeAsync();
    }
}