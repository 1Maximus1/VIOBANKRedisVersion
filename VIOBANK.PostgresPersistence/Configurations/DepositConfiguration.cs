using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class DepositConfiguration : IEntityTypeConfiguration<Deposit>
    {
        public void Configure(EntityTypeBuilder<Deposit> builder)
        {
            builder.HasKey(d => d.DepositId);

            builder.Property(d => d.Amount)
                .IsRequired();

            builder.Property(d => d.InitialAmount)
                .IsRequired();

            builder.Property(d => d.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(d => d.DurationMonths)
                .IsRequired();

            builder.Property(d => d.InterestRate)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.HasOne(d => d.Card)
                .WithMany(c => c.Deposits)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}
