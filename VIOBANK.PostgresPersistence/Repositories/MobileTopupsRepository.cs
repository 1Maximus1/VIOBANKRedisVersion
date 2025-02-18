using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class MobileTopupsRepository : IMobileTopupStore
    {
        private readonly VIOBANKDbContext _context;

        public MobileTopupsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<MobileTopup>> GetByUserId(int userId)
        {
            return await _context.MobileTopups.AsNoTracking()
                .Where(m => m.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<MobileTopup> GetById(int topupId)
        {
            return await _context.MobileTopups.AsNoTracking().FirstOrDefaultAsync(t=> t.TopupId.Equals(topupId));
        }

        public async Task Add(MobileTopup topup)
        {
            _context.MobileTopups.Add(topup);
            await _context.SaveChangesAsync();
        }
    }
}
