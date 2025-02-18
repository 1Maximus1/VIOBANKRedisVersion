
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.PostgresPersistence.Configurations;

namespace VIOBANK.PostgresPersistence
{
    public class VIOBANKDbContext: DbContext
    {
        public VIOBANKDbContext(DbContextOptions<VIOBANKDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<MobileTopup> MobileTopups { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<DepositTransaction> DepositTransactions { get; set; }
        public DbSet<WithdrawnDeposit> WithdrawnDeposits { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new CardConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new DepositConfiguration());
            modelBuilder.ApplyConfiguration(new MobileTopupConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DepositTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawnDepositConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
