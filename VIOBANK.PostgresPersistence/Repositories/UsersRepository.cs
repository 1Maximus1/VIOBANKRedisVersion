using VIOBANK.Domain.Filters;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;
using Microsoft.EntityFrameworkCore;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class UsersRepository : IUserStore
    {
        private readonly VIOBANKDbContext _context;

        public UsersRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IReadOnlyList<User>> GetByFilter(UserFilter filter)
        {
            var query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(u => u.Email.Contains(filter.Email));

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(u => u.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Surname))
                query = query.Where(u => u.Surname.Contains(filter.Surname));

            if (!string.IsNullOrWhiteSpace(filter.Phone))
                query = query.Where(u => u.Phone.Contains(filter.Phone));

            if (!string.IsNullOrWhiteSpace(filter.IdCard))
                query = query.Where(u => u.IdCard.Contains(filter.IdCard));

            if (!string.IsNullOrWhiteSpace(filter.TaxNumber))
                query = query.Where(u => u.TaxNumber.Contains(filter.TaxNumber));

            if (filter.CreatedAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);

            return await query.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> GetByCardNumber(string cardNumber)
        {
            var card = await _context.Cards
                .Include(c => c.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

            return card?.Account?.User;
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
