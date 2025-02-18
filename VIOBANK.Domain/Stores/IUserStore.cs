using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Filters;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IUserStore
    {
        Task<IReadOnlyList<User>> GetByFilter(UserFilter filter); // Отримання користувачів за фільтром
        Task<User> GetById(int id); // Отримання користувача за ID
        Task<User> GetByEmail(string email); // Отримання користувача за email
        Task<User> GetByCardNumber(string cardNumber);
        Task Add(User user); // Додавання нового користувача
        Task Update(User user); // Оновлення існуючого користувача
        Task Delete(int id); // Видалення користувача
    }
}
