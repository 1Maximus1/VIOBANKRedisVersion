using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class DepositsRepository : IDepositStore
    {
        private readonly VIOBANKDbContext _context;

        public DepositsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Deposit>> GetByUserId(int userId)
        {
            return await _context.Deposits.AsNoTracking()
                .Where(d => d.Card.Account.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<Deposit> GetById(int depositId)
        {
            return await _context.Deposits
                .Include(d => d.Card)
                .ThenInclude(c => c.Account) 
                .FirstOrDefaultAsync(d => d.DepositId == depositId);
        }


        public async Task Add(Deposit deposit)
        {
            _context.Deposits.Add(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Deposit deposit)
        {
            _context.Deposits.Update(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int depositId)
        {
            var deposit = await _context.Deposits.FindAsync(depositId);
            _context.Deposits.Remove(deposit);
            await _context.SaveChangesAsync();
        }

    }
}
