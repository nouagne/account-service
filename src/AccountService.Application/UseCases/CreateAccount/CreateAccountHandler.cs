using AccountService.Application.Contracts.Common;
using AccountService.Application.Contracts.CQRS;
using AccountService.Application.Contracts.Repositories;
using AccountService.Application.Contracts.Security;
using AccountService.Domain.Entities;

namespace AccountService.Application.UseCases.CreateAccount;

public sealed class CreateAccountHandler(
    IAccountRepository repo,
    IPasswordHasher hasher,
    IClock clock
) : ICommandHandler<CreateAccountCommand, CreateAccountResult>
{
    public async Task<CreateAccountResult> Handle(CreateAccountCommand cmd, CancellationToken ct)
    {
        var email = cmd.Email.Trim().ToLowerInvariant();

        var account = new Account
        {
            AccountId = Guid.NewGuid(),
            Email = email,
            PasswordHash = hasher.Hash(cmd.Password),
            Firstname = cmd.FirstName?.Trim(),
            Lastname = cmd.LastName?.Trim(),
            CreatedAt = clock.UtcNow,
            UpdatedAt = clock.UtcNow
        };
        
        await repo.AddAsync(account, ct);

        return new CreateAccountResult(account.AccountId);
    }
}