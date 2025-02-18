using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class SettingsConfiguration : IEntityTypeConfiguration<Settings>
    {
        public void Configure(EntityTypeBuilder<Settings> builder)
        {
            // Визначення Primary Key
            builder.HasKey(s => s.SettingId);

            // Налаштування полів
            builder.Property(s => s.NotificationsEmail)
                .IsRequired();

            builder.Property(s => s.NotificationsSms)
                .IsRequired();

            builder.Property(s => s.Language)
                .HasMaxLength(20)
                .IsRequired();

            // Зв’язок One-to-One (Користувач → Налаштування)
            builder.HasOne(s => s.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<Settings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
