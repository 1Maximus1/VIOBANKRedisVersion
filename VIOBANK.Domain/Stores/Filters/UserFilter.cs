using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIOBANK.Domain.Filters
{
    public class UserFilter
    {
        public string Name { get; set; } // Ім'я користувача (необов'язковий параметр)
        public string Surname { get; set; }
        public string Email { get; set; } // Email користувача
        public string Phone { get; set; } // Номер телефону
        public string IdCard { get; set; }  // Добавлено
        public string TaxNumber { get; set; } // Добавлено
        public bool? IsVerified { get; set; } // Фільтр за верифікацією
        public DateTime? CreatedAfter { get; set; } // Користувачі, створені після цієї дати
        public DateTime? CreatedBefore { get; set; } // Користувачі, створені до цієї дати
    }
}
