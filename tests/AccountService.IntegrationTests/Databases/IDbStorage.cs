namespace AccountService.IntegrationTests.Databases;

public interface IDbStorage : IAsyncDisposable
{
    /// Lance le service (conteneur/émulateur), crée DB/tables/containers, indexes… 
    Task InitializeAsync();

    /// Nettoie entre tests (truncate/drop/db reset)
    Task ResetAsync();

    /// Variables d'env à injecter avant de démarrer l'API (provider, cnx, endpoint…)
    IReadOnlyDictionary<string, string> GetEnvironment();
    
    String GetConnectionString();
}