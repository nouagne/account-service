using AccountService.Application.Contracts.Repositories;
using AccountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.Data.Postgres.Repositories;

public class AccountRepository(AppDbContext dbContext) : IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == id, ct);

    public Task<Account?> GetByEmailAsync(string email, CancellationToken ct)
        => dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);
    
    public async Task AddAsync(Account account, CancellationToken ct)
    {
        await dbContext.Accounts.AddAsync(account, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Account account, CancellationToken ct)
    {
        dbContext.Accounts.Update(account);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var account = await GetByIdAsync(id, ct);
        if (account == null) return;
        
        account.Status = "Deactivated";
        account.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(ct);
    }
}