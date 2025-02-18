using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class SettingsRepository : ISettingsStore
    {
        private readonly VIOBANKDbContext _context;

        public SettingsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<Settings> GetByUserId(int userId)
        {
            return await _context.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.UserId.Equals(userId));
        }

        public async Task Update(Settings settings)
        {
            _context.Settings.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}
