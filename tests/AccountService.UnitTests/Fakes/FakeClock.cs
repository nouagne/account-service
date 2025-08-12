using AccountService.Application.Contracts.Common;

namespace AccountService.UnitTests.Fakes;

public class FakeClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset UtcNow { get; } = now;
}
