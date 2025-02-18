using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class DepositTransactionConfiguration : IEntityTypeConfiguration<DepositTransaction>
    {
        public void Configure(EntityTypeBuilder<DepositTransaction> builder)
        {
            // Устанавливаем первичный ключ
            builder.HasKey(t => t.TransactionId);

            // Указываем, что поле `Amount` обязательно
            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Два знака после запятой

            // Поле `CreatedAt` обязательно
            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone") // PostgreSQL поддержка времени с учетом часового пояса
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Настраиваем внешний ключ на `Deposit`
            builder.HasOne(t => t.Deposit)
                .WithMany(d => d.DepositTransactions) // Связь один-ко-многим (один депозит, много пополнений)
                .HasForeignKey(t => t.DepositId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении депозита удаляются и связанные транзакции
        }
    }

}
