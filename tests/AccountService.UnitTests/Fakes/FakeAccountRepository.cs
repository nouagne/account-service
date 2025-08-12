using AccountService.Application.Contracts.Repositories;
using AccountService.Domain.Entities;

namespace AccountService.UnitTests.Fakes;

public class FakeAccountRepository : IAccountRepository
{
    private readonly List<Account> _accounts = new();
    
    public Task<Account?> GetByIdAsync(Guid accountId, CancellationToken ct)
        => Task.FromResult(_accounts.FirstOrDefault(a => a.AccountId == accountId));
    
    public Task<Account?> GetByEmailAsync(string email, CancellationToken ct)
        => Task.FromResult(_accounts.FirstOrDefault(a => a.Email == email));

    public Task AddAsync(Account account, CancellationToken ct)
    {
        _accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account, CancellationToken ct) => Task.CompletedTask;
    public Task DeleteAsync(Guid id, CancellationToken ct)
    {
        _accounts.RemoveAll(a => a.AccountId == id);
        return Task.CompletedTask;
    }
}