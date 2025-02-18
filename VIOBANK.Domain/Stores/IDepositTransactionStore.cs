
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IDepositTransactionStore
    {
        Task<decimal> GetTotalTopUpForMonth(int depositId, DateTime startOfMonth);

        Task<List<DepositTransaction>> GetTopUpsByDepositId(int depositId);

        // Получить последнее пополнение депозита
        Task<DepositTransaction> GetLastTopUp(int depositId);

        // Проверка, можно ли пополнить депозит (не позднее 3 месяцев до окончания срока)
        Task<bool> CanTopUpDeposit(int depositId, decimal amount, int maxMonthlyLimit);

        // Добавить пополнение депозита
        Task<bool> AddTopUp(DepositTransaction transaction);
        Task Add(DepositTransaction transaction);

    }
}
