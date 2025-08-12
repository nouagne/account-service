using System.Net.Http.Json;
using AccountService.IntegrationTests.Databases;

namespace AccountService.IntegrationTests;

public class AccountsApiTests : IntegrationTestDatabase
{
    [Fact]
    public async Task Create_and_get_roundtrip()
    {
        var req = new { email="user2@example.com", password="Secret123!", firstName="Nico", lastName="O" };
        var create = await Client.PostAsJsonAsync("/api/accounts", req);
        create.EnsureSuccessStatusCode();

        var location = create.Headers.Location!.ToString();
        var get = await Client.GetAsync(location);
        get.EnsureSuccessStatusCode();
    }
}