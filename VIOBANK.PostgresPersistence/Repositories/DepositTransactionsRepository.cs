using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class DepositTransactionsRepository : IDepositTransactionStore
    {
        private readonly VIOBANKDbContext _context;

        public DepositTransactionsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalTopUpForMonth(int depositId, DateTime startOfMonth)
        {
            return await _context.DepositTransactions
                .Where(t => t.DepositId == depositId && t.CreatedAt >= startOfMonth)
                .Select(t => t.Amount)
                .SumAsync();
        }

        public async Task<List<DepositTransaction>> GetTopUpsByDepositId(int depositId)
        {
            return await _context.DepositTransactions
                .Where(t => t.DepositId == depositId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<DepositTransaction> GetLastTopUp(int depositId)
        {
            return await _context.DepositTransactions
                .Where(t => t.DepositId == depositId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CanTopUpDeposit(int depositId, decimal amount, int maxMonthlyLimit)
        {
            var deposit = await _context.Deposits.FindAsync(depositId);
            if (deposit == null)
                return false;

            DateTime maturityDate = deposit.CreatedAt.AddMonths(deposit.DurationMonths);
            int remainingMonths = ((maturityDate.Year - DateTime.UtcNow.Year) * 12) + (maturityDate.Month - DateTime.UtcNow.Month);

            // Проверка срока пополнения
            if (remainingMonths < 3)
                return false;

            // Проверка лимита на пополнение за месяц
            DateTime startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var totalTopUp = await GetTotalTopUpForMonth(depositId, startOfMonth);
            if ((totalTopUp + amount) > maxMonthlyLimit)
                return false;

            return true;
        }

        public async Task<bool> AddTopUp(DepositTransaction transaction)
        {
            try
            {
                _context.DepositTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding deposit top-up: {ex.Message}");
                return false;
            }
        }

        public async Task Add(DepositTransaction transaction)
        {
            await _context.DepositTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
