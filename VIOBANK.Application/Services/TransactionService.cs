using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Logging;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.Application.Services
{
    public class TransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private readonly ITransactionStore _transactionStore;
        private readonly CardService _cardService;
        private readonly AccountService _accountService;
        private readonly CurrencyExchangeService _currencyExchangeService;

        public TransactionService(ILogger<TransactionService> logger, ITransactionStore transactionStore, CardService cardService, AccountService accountService, CurrencyExchangeService currencyExchangeService)
        {
            _logger = logger;
            _transactionStore = transactionStore;
            _cardService = cardService;
            _accountService = accountService;
            _currencyExchangeService = 
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsByAccountId(int accountId)
        {
            _logger.LogInformation($"Receiving transactions for an account {accountId}");
            return await _transactionStore.GetByAccountId(accountId);
        }

        public async Task<Transaction> GetTransactionById(int transactionId)
        {
            _logger.LogInformation($"Receiving a transaction with ID: {transactionId}");
            return await _transactionStore.GetById(transactionId);
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            _logger.LogInformation($"Adding a new transaction from {transaction.FromCardId} to {transaction.ToCardId} on amount {transaction.Amount}");

            await _transactionStore.Add(transaction);
            return transaction;
        }

        public async Task<IReadOnlyList<Transaction>> GetAllTransactions()
        {
            return await _transactionStore.GetAll();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllTransactions(int userId)
        {
            return await _transactionStore.GetAllByUserId(userId);
        }

        public async Task Accomplish(Transaction transaction)
        {
            if (transaction.FromCard.Balance < transaction.Amount)
            {
                throw new InvalidOperationException("Not enough money on card");
            }

            decimal amountToTransfer = transaction.Amount;

            if (transaction.FromCard.Account.Currency != transaction.ToCard.Account.Currency)
            {
                decimal exchangeRate = await _currencyExchangeService.GetExchangeRateAsync(
                    transaction.FromCard.Account.Currency,
                    transaction.ToCard.Account.Currency
                );

                amountToTransfer = transaction.Amount * exchangeRate;
            }

            transaction.FromCard.Balance -= transaction.Amount;
            transaction.ToCard.Balance += amountToTransfer;

            await _cardService.UpdateCard(transaction.FromCard);
            await _cardService.UpdateCard(transaction.ToCard);
        }

    }
}
