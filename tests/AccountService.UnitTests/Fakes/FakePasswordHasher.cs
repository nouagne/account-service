using AccountService.Application.Contracts.Security;

namespace AccountService.UnitTests.Fakes;

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string plaintext) => $"HASH::{plaintext}";
    public bool Verify(string hash, string plaintext) => hash == $"HASH::{plaintext}";
}