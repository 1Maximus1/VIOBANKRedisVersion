using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface ITransactionStore
    {
        Task<IReadOnlyList<Transaction>> GetByAccountId(int accountId); // Отримання транзакцій за ID рахунку
        Task<IReadOnlyList<Transaction>> GetAllByUserId(int userId);
        Task<Transaction> GetById(int transactionId); // Отримання транзакції за ID
        Task<Transaction> Add(Transaction transaction); // Додавання нової транзакції
        Task<IReadOnlyList<Transaction>> GetAll();

    }
}
