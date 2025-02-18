using Microsoft.EntityFrameworkCore;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.PostgresPersistence.Repositories
{
    public class CardsRepository : ICardStore
    {
        private readonly VIOBANKDbContext _context;

        public CardsRepository(VIOBANKDbContext context)
        {
            _context = context;
        }

        public async Task<Card> Add(Card card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task Delete(int cardId)
        {
            var card = await _context.Cards.FindAsync(cardId);
            if (card != null)
            {
                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<Card>> GetAll()
        {
            return await _context.Cards.AsNoTracking().ToListAsync();
        }

        public async Task<Card> GetByAccountId(int accountId)
        {
            return await _context.Cards
                .Where(c => c.AccountId == accountId)
                .FirstOrDefaultAsync();
        }


        public async Task<Card> GetByCardNumber(string cardNumber)
        {
            return await _context.Cards
                .Include(c => c.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }

        public async Task<Card> GetById(int cardId)
        {
            return await _context.Cards.AsNoTracking().FirstAsync(c=>c.CardId == cardId);
        }

        public async Task<IReadOnlyList<Card>> GetByUserId(int userId)
        {
            return await _context.Cards.AsNoTracking()
                .Where(c => c.Account.User.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<string> GetCardPasswordByCardId(int cardId)
        {
            var cardPassword = await _context.Cards
                .Where(c => c.CardId == cardId)
                .Select(c => c.CardPassword)
                .FirstOrDefaultAsync();

            return cardPassword ?? "Card not found"; 
        }

        public async Task<string> GetCardPasswordByUserId(int userId)
        {
            var cardPassword = await _context.Cards
                .Where(c => c.Account.UserId == userId)
                .Select(c => c.CardPassword)
                .FirstOrDefaultAsync();


            return cardPassword ?? "Card not found"; 
        }

        public async Task Update(Card card)
        {
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(List<Card> cards)
        {
            _context.Cards.UpdateRange(cards);
            await _context.SaveChangesAsync();
        }
    }
}
