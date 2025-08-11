using AccountService.Application.Contracts.Security;

namespace AccountService.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    public bool Verify(string hash, string password) => BCrypt.Net.BCrypt.Verify(password, hash);
}