using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;

namespace VIOBANK.PostgresPersistence.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            // Визначення Primary Key
            builder.HasKey(c => c.ContactId);

            // Встановлення обов'язкових параметрів
            builder.Property(c => c.ContactName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.ContactPhone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.ContactCard)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(u => u.ContactCard).IsUnique();

            // Зв’язок Many-to-One (Користувач → Контакти)
            builder.HasOne(c => c.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
