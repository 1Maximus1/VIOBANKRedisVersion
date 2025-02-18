using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IAccountStore
    {
        Task<IReadOnlyList<Account>> GetByUserId(int userId); // Отримання всіх рахунків користувача
        Task<Account> GetByAccountNumber(string accountNumber); // Отримання рахунку за номером
        Task Add(Account account); // Додавання нового рахунку
        Task Update(Account account); // Оновлення рахунку
        Task Delete(int accountId); // Видалення рахунку
        Task<Account> GetByType(int userId, string type);
        Task<Account> GetByCardNumber(string cardNumber);
        Task<Account> GetById(int accountId);
    }
}
