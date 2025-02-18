using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class AccountsRepository : IAccountStore
    {
        private readonly VIOBANKDbContext _context;

        public AccountsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task Add(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int accountId)
        {
                var account = await _context.Accounts
        .Include(a => a.Cards)
            .ThenInclude(c => c.TransactionsFrom) // Исходящие транзакции
        .Include(a => a.Cards)
            .ThenInclude(c => c.TransactionsTo)   // Входящие транзакции
        .Include(a => a.Cards)
        .ThenInclude(c => c.Deposits)
        .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account != null)
            {
                // Удаляем транзакции карт
                foreach (var card in account.Cards)
                {
                    _context.Transactions.RemoveRange(card.TransactionsFrom);
                    _context.Transactions.RemoveRange(card.TransactionsTo);
                    _context.Deposits.RemoveRange(card.Deposits);
                }

                // Удаляем карты
                _context.Cards.RemoveRange(account.Cards);

                // Удаляем сам аккаунт
                _context.Accounts.Remove(account);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Account> GetByAccountNumber(string accountNumber)
        {
            return await _context.Accounts.AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task Update(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Account>> GetByUserId(int userId)
        {
            return await _context.Accounts.AsNoTracking()
                .Where(a => a.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<Account> GetByType(int userId, string type)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.Currency == type)
                .FirstOrDefaultAsync();
        }

        public async Task<Account> GetByCardNumber(string cardNumber)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Cards.Any(c => c.CardNumber == cardNumber));
        }

        public async Task<Account> GetById(int accountId)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstAsync(a=>a.AccountId == accountId);
        }
    }
}
