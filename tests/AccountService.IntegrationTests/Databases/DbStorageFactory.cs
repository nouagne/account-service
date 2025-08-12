using AccountService.IntegrationTests.Fixtures;

namespace AccountService.IntegrationTests.Databases;

public static class DbStorageFactory
{
    public static IDbStorage Create()
    {
        var storageKind = (Environment.GetEnvironmentVariable("TEST_BACKEND")?.ToLowerInvariant() ?? "postgres")
            .ToLowerInvariant();


        return storageKind switch
        {
            //Add databases fixtures here
            "postgres" => new PostgresFixture(),   // SQL (EF Core côté app)
            _ => throw new NotSupportedException($"TEST_BACKEND={storageKind} not supported")
        };
        
    }
}