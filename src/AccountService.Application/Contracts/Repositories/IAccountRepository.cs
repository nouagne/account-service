using AccountService.Domain.Entities;

namespace AccountService.Application.Contracts.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId, CancellationToken ct);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(Account account, CancellationToken ct);
    Task UpdateAsync(Account account, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct); // soft delete dans l’impl
}