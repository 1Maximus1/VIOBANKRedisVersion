using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class WithdrawnDepositConfiguration : IEntityTypeConfiguration<WithdrawnDeposit>
    {
        public void Configure(EntityTypeBuilder<WithdrawnDeposit> builder)
        {
            builder.HasKey(wd => wd.WithdrawnDepositId);

            builder.Property(wd => wd.Amount)
                .IsRequired();

            builder.Property(wd => wd.InterestEarned)
                .IsRequired();

            builder.Property(wd => wd.TotalAmount)
                .IsRequired();

            builder.Property(wd => wd.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(wd => wd.CreatedAt)
                .IsRequired();

            builder.Property(wd => wd.WithdrawnAt)
                .IsRequired();

            builder.HasOne(wd => wd.User)
               .WithMany(u => u.WithdrawnDeposits)
               .HasForeignKey(wd => wd.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
