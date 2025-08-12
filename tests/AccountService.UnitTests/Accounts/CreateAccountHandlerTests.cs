using AccountService.Application.UseCases.CreateAccount;
using AccountService.UnitTests.Fakes;

namespace AccountService.UnitTests.Accounts;

public class CreateAccountHandlerTests
{
    [Fact]
    public async Task Handle_Creates_Account_With_Normalized_Email_And_Timestamps()
    {
        // Arrange
        var repo   = new FakeAccountRepository();
        var clock  = new FakeClock(new DateTimeOffset(2025, 8, 11, 12, 0, 0, TimeSpan.Zero));
        var hasher = new FakePasswordHasher();
        var handler = new CreateAccountHandler(repo, hasher, clock);

        var cmd = new CreateAccountCommand
        {
            Email = "User@Example.Com",
            Password = "secret123",
            FirstName = "Nicolas",
            LastName  = "Ouagne"
        };

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.AccountId);

        var saved = await repo.GetByIdAsync(result.AccountId, CancellationToken.None);
        Assert.NotNull(saved);
        Assert.Equal("user@example.com", saved.Email);
        Assert.Equal("HASH::secret123", saved.PasswordHash);
        Assert.Equal(clock.UtcNow, saved.CreatedAt);
        Assert.Equal(clock.UtcNow, saved.UpdatedAt);
    }
}