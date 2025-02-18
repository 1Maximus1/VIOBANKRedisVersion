using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface ICardStore
    {
        Task<IReadOnlyList<Card>> GetByUserId(int userId); // Отримання всіх карток користувача
        Task<Card> GetByCardNumber(string cardNumber); // Отримання картки за номером
        Task<Card> Add(Card card); // Додавання нової картки
        Task Update(Card card); // Оновлення картки
        Task Delete(int cardId); // Видалення картки
        Task<IReadOnlyList<Card>> GetAll();
        Task<string> GetCardPasswordByCardId(int cardId);
        Task<string> GetCardPasswordByUserId(int userId);
        Task<Card> GetById(int cardId);
        Task<Card> GetByAccountId(int accountId);
        Task UpdateRange(List<Card> cards);
    }
}
