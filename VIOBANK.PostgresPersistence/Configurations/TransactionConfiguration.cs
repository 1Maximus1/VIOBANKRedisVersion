using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.Amount)
                .IsRequired();

            builder.Property(t => t.CurrencyFrom)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(t => t.CurrencyTo)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasOne(t => t.FromCard)
                .WithMany(c => c.TransactionsFrom)
                .HasForeignKey(t => t.FromCardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.ToCard)
                .WithMany(c => c.TransactionsTo)
                .HasForeignKey(t => t.ToCardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
