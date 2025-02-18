using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Визначення Primary Key
            builder.HasKey(a => a.AccountId);

            // Унікальний номер рахунку
            builder.Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(28);
            builder.HasIndex(a => a.AccountNumber).IsUnique();

            // Баланс та валюта
            builder.Property(a => a.Balance)
                .IsRequired();

            builder.Property(a => a.Currency)
                .HasMaxLength(10)
                .IsRequired();

            // Зв’язок з користувачем (Many-to-One)
            builder.HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Зв’язок з картами (One-to-Many)
            builder.HasMany(a => a.Cards)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
