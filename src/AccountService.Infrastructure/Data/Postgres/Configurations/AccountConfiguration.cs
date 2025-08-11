using AccountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Infrastructure.Data.Postgres.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(x => x.AccountId);
           
        builder.Property(x => x.Email).IsRequired().HasColumnType("citext");
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired();
           
        builder.Property(x => x.Status).HasDefaultValue("Active");
        builder.Property(x => x.Locale).HasDefaultValue("fr-FR");
        builder.Property(x => x.Timezone).HasDefaultValue("Europe/Paris");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
    }
}