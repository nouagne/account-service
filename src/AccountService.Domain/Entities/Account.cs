namespace AccountService.Domain.Entities;

public class Account
{
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string Locale { get; set; } = "fr-FR";
    public string Timezone { get; set; } = "Europe/Paris";
    public string Status { get; set; } = "Active";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}