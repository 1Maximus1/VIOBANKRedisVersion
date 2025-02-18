using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IDepositStore
    {
        Task<IReadOnlyList<Deposit>> GetByUserId(int userId); // Отримання всіх депозитів користувача
        Task<Deposit> GetById(int depositId); // Отримання депозиту за ID
        Task Add(Deposit deposit); // Додавання нового депозиту
        Task Update(Deposit deposit); // Оновлення депозиту
        Task Delete(int depositId);
    }
}
