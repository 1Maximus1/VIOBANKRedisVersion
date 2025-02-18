using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface ISettingsStore
    {
        Task<Settings> GetByUserId(int userId); // Отримання налаштувань для користувача
        Task Update(Settings settings); // Оновлення налаштувань

    }
}
