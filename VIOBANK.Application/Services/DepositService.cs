using Microsoft.Extensions.Logging;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.Application.Services
{
    public class DepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly IDepositStore _depositStore;
        private readonly CardService _cardService;
        private readonly AccountService _accountService;
        private readonly IDepositTransactionStore _depositTransactionStore;
        private readonly WithdrawnDepositService _withdrawnDepositService;

        public DepositService(ILogger<DepositService> logger, IDepositStore depositStore, CardService cardService, AccountService accountService, IDepositTransactionStore depositTransactionStore, WithdrawnDepositService withdrawnDepositService)
        {
            _logger = logger;
            _depositStore = depositStore;
            _cardService = cardService;
            _accountService = accountService;
            _depositTransactionStore = depositTransactionStore;
            _withdrawnDepositService = withdrawnDepositService;
        }

        public async Task<IReadOnlyList<Deposit>> GetDepositsByUserId(int userId)
        {
            _logger.LogInformation($"Receiving user deposits with ID: {userId}");
            return await _depositStore.GetByUserId(userId);
        }

        public async Task<Deposit> GetDepositById(int depositId)
        {
            _logger.LogInformation($"Receiving a deposit with ID: {depositId}");
            return await _depositStore.GetById(depositId);
        }

        public async Task AddDeposit(Deposit deposit)
        {
                _logger.LogInformation($"Adding a new user deposit");
                await _depositStore.Add(deposit);

        }

        public async Task<bool> UpdateDeposit(Deposit deposit)
        {
            try
            {
                _logger.LogInformation($"Updating a deposit with ID: {deposit.DepositId}");
                await _depositStore.Update(deposit);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating deposit: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool Success, string Message, decimal NewAmount)> TopUpDeposit(int depositId, decimal amount)
        {
            var deposit = await _depositStore.GetById(depositId);
            if (deposit == null)
            {
                return (false, "Deposit not found.", 0);
            }

            // Проверка на оставшиеся месяцы до конца депозита
            var maturityDate = deposit.CreatedAt.AddMonths(deposit.DurationMonths);
            var remainingMonths = ((maturityDate.Year - DateTime.UtcNow.Year) * 12) + (maturityDate.Month - DateTime.UtcNow.Month);

            if (remainingMonths < 3)
            {
                return (false, "Top-up not allowed less than 3 months before maturity.", 0);
            }

            // Проверка лимита пополнения (1000 € в месяц)
            decimal monthlyTopUpLimit = deposit.InitialAmount; // Используем первоначальную сумму депозита

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var totalToppedUpThisMonth = await _depositTransactionStore.GetTotalTopUpForMonth(depositId, startOfMonth);

            if (totalToppedUpThisMonth + amount > monthlyTopUpLimit)
            {
                return (false, $"Top-up exceeds monthly limit of {monthlyTopUpLimit} €.", 0);
            }

            // **Добавляем запись о пополнении в таблицу `DepositTransactions`**
            var transaction = new DepositTransaction
            {
                DepositId = depositId,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            await _depositTransactionStore.Add(transaction);

            // Обновляем баланс депозита
            deposit.Amount += amount;
            await _depositStore.Update(deposit);

            return (true, "Deposit topped up successfully.", deposit.Amount);
        }



        public async Task<(bool Success, string Message, decimal AmountTransferred)> PayoutDeposit(int depositId)
        {
            var deposit = await _depositStore.GetById(depositId);
            if (deposit == null || (deposit.CreatedAt.AddMonths(deposit.DurationMonths) > DateTime.UtcNow))
            {
                return (false, "Deposit is not yet mature for payout.", 0);
            }

            var totalAmount = deposit.Amount;
            var linkedCard = await _cardService.GetCardByNumber(deposit.Card.CardNumber);
            if (linkedCard == null)
            {
                return (false, "Linked card not found.", 0);
            }

            linkedCard.Balance += totalAmount;
            await _cardService.UpdateCard(linkedCard);
            await _depositStore.Delete(deposit.DepositId);

            return (true, "Deposit payout completed", totalAmount);
        }

        public async Task<(bool Success, string Message, decimal AmountPaid)> WithdrawDeposit(int depositId, int userId)
        {
            var deposit = await _depositStore.GetById(depositId);
            if (deposit == null)
            {
                return (false, "Deposit not found.", 0);
            }
            if (deposit.Card.Account.UserId != userId)
            {
                return (false, "You are not allowed to withdraw this deposit.", 0);
            }

            var account = await _accountService.GetAccountById(deposit.Card.AccountId);
            if (account == null)
            {
                return (false, "Linked account not found.", 0);
            }

            // Перевіряємо, чи термін закінчився
            var maturityDate = deposit.CreatedAt.AddMonths(deposit.DurationMonths);
            if (DateTime.UtcNow < maturityDate)
            {
                return (false, "Deposit maturity period not reached.", 0);
            }

            // Обчислення загальної суми (депозит + відсотки)
            decimal interestEarned = (deposit.Amount * deposit.InterestRate / 100 * deposit.DurationMonths);
            decimal totalAmount = deposit.Amount + interestEarned;


            // Додаємо кошти на картку
            deposit.Card.Balance += totalAmount;
            await _cardService.UpdateCard(deposit.Card);

            // Создаем запись о снятом депозите
            var withdrawnDeposit = new WithdrawnDeposit
            {
                UserId = userId,
                Amount = deposit.Amount,
                InterestEarned = interestEarned,
                TotalAmount = totalAmount,
                Currency = deposit.Currency,
                CreatedAt = deposit.CreatedAt,
                WithdrawnAt = DateTime.UtcNow
            };
            await _withdrawnDepositService.AddWithdrawnDeposit(withdrawnDeposit);

            await _depositStore.Delete(deposit.DepositId);

            return (true, "Deposit withdrawn successfully.", totalAmount);
        }
    }
}
