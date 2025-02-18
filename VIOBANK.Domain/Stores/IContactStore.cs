using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IContactStore
    {
        Task<IReadOnlyList<Contact>> GetByUserId(int userId); // Отримання всіх контактів для користувача
        Task<Contact> GetById(int contactId); // Отримання контакту за ID
        Task Add(Contact contact); // Додавання нового контакту
        Task Update(Contact contact); // Оновлення існуючого контакту
        Task Delete(int contactId); // Видалення контакту
        Task<bool> ContactExists(string cardNumber);
    }
}
