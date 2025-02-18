using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class ContactsRepository : IContactStore
    {
        private readonly VIOBANKDbContext _context;

        public ContactsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Contact>> GetByUserId(int userId)
        {
            return await _context.Contacts.AsNoTracking()
                .Where(c => c.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<Contact> GetById(int contactId)
        {
            return await _context.Contacts.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ContactId.Equals(contactId));
        }

        public async Task Add(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Contact contact)
        {
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ContactExists(string cardNumber)
        {
            return await _context.Contacts.AnyAsync(c => c.ContactCard == cardNumber);
        }
    }
}
