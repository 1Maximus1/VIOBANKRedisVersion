using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class TransactionsRepository : ITransactionStore
    {
        private readonly VIOBANKDbContext _context;

        public TransactionsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Transaction>> GetByAccountId(int accountId)
        {
            return await _context.Transactions.AsNoTracking()
                .Where(t => t.FromCard.Account.Equals(accountId) || t.ToCard.Account.Equals(accountId))
                .ToListAsync();
        }

        public async Task<Transaction> GetById(int transactionId)
        {
            return await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId.Equals(transactionId));

        }

        public async Task<Transaction> Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<IReadOnlyList<Transaction>> GetAll()
        {
            return await _context.Transactions
                .Include(t => t.FromCard)
                .ThenInclude(c => c.Account)
                .Include(t => t.ToCard)
                .ThenInclude(c => c.Account)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllByUserId(int userId)
        {
            return await _context.Transactions
                .Include(t => t.FromCard)
                    .ThenInclude(c => c.Account)
                .Include(t => t.ToCard)
                    .ThenInclude(c => c.Account)
                .Where(t => t.FromCard.Account.UserId == userId || t.ToCard.Account.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
