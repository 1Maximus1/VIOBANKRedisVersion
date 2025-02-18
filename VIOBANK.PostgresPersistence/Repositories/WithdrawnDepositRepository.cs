
using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class WithdrawnDepositRepository : IWithdrawnDepositStore
    {
        private readonly VIOBANKDbContext _context;

        public WithdrawnDepositRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task Add(WithdrawnDeposit withdrawnDeposit)
        {
            await _context.WithdrawnDeposits.AddAsync(withdrawnDeposit);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WithdrawnDeposit>> GetByUserId(int userId)
        {
            return await _context.WithdrawnDeposits
                .Where(wd => wd.UserId == userId)
                .ToListAsync();
        }

    }
}
