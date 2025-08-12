using System.Net;
using System.Net.Http.Json;
using AccountService.API.Features.Accounts.Create;
using AccountService.API.Features.Accounts.Get;

namespace AccountService.IntegrationTests;

public class AccountsApiTests : IAsyncLifetime
{
    private readonly PostgresFixture _pg = new();
    private string _cs = null!;
    private HttpClient _client = null!;
    private TestApiFactory _factory = null!;
    private string _dbName = null!;

    public async Task InitializeAsync()
    {
        _cs = await _pg.CreateTestDatabaseAsync("accountsvc");
        _dbName = new Npgsql.NpgsqlConnectionStringBuilder(_cs).Database!;
        Console.WriteLine($"[TEST] DB créée: {_dbName}");
        
        _factory = new TestApiFactory(_cs);
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _pg.DropDatabaseAsync(_cs);
        await _factory.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task Post_then_Get_returns_account()
    {
        var req = new CreateAccountRequest
        {
            Email = "User@Example.com",
            Password = "secret123",
            FirstName = "Nico",
            LastName = "O"
        };

        var post = await _client.PostAsJsonAsync("/api/Accounts", req);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<CreateAccountResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.AccountId);

        var get = await _client.GetAsync($"/api/Accounts/{created.AccountId}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var dto = await get.Content.ReadFromJsonAsync<GetAccountResponse>();
        Assert.NotNull(dto);
        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal("Nico", dto.FirstName);
    }

    [Fact]
    public async Task Get_unknown_id_returns_404()
    {
        var res = await _client.GetAsync($"/api/Accounts/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}