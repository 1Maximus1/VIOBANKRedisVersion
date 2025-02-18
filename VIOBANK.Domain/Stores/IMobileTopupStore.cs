using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IMobileTopupStore
    {
        Task<IReadOnlyList<MobileTopup>> GetByUserId(int userId); // Отримання історії поповнень для користувача
        Task<MobileTopup> GetById(int topupId); // Отримання конкретного поповнення за ID
        Task Add(MobileTopup topup); // Додавання нового поповнення
    }
}
