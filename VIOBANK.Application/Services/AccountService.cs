using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;
using VIOBANK.Infrastructure;

namespace VIOBANK.Application.Services
{
    public class AccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IAccountStore _accountStore;

        public AccountService(ILogger<AccountService> logger, IAccountStore accountStore)
        {
            _logger = logger;
            _accountStore = accountStore;
        }

        public async Task<Account> GetAccountByNumber(string accountNumber)
        {
            _logger.LogInformation($"Request account with number: {accountNumber}");
            return await _accountStore.GetByAccountNumber(accountNumber);
        }

        public async Task<IReadOnlyList<Account>> GetAccountsByUserId(int userId)
        {
            _logger.LogInformation($"Query all user accounts with ID: {userId}");
            return await _accountStore.GetByUserId(userId);
        }

        public async Task CreateAccount(Account account)
        {
            _logger.LogInformation($"Create an account: {account.AccountNumber}");
            await _accountStore.Add(account);
        }

        public async Task UpdateAccount(Account account)
        {
            _logger.LogInformation($"Account update: {account.AccountNumber}");
            await _accountStore.Update(account);
        }

        public async Task DeleteAccount(int accountId)
        {
            _logger.LogInformation($"Deleting an account with ID: {accountId}");
            await _accountStore.Delete(accountId);
        }

        public async Task Add(Account account)
        {
            await _accountStore.Add(account);
        }

        public async Task<Account> GetAccountByType(int userId, string type)
        {
            return await _accountStore.GetByType(userId, type);
        }

        public async Task<Account> GetAccountById(int accountId)
        {
            _logger.LogInformation($"Geting account with id: {accountId}");
            return await _accountStore.GetById(accountId);
        }

        public async Task<Account> GetAccountByCardNumber(string cardNumber)
        {
            return await _accountStore.GetByCardNumber(cardNumber);
        }

        public Task<Account> GenerateAccount(User user, decimal balance, string currency)
        {
            return Task.FromResult(new Account
            {
                UserId = user.UserId,
                AccountNumber = GenerateAccountNumber(),
                Balance = balance,
                Currency = currency,
                CreatedAt = DateTime.UtcNow
            });

        }

        public string GenerateAccountNumber()
        {
            string countryCode = "UA";  
            string bankCode = "228228"; 
            string accountNumber = new Random().Next(100000000, 999999999).ToString() +
                                   new Random().Next(100000000, 999999999).ToString();

            string controlDigits = "00"; // Контрольные цифры (их можно рассчитывать)

            return $"{countryCode}{controlDigits}{bankCode}{accountNumber}";
        }

        public async Task Logout(string email)
        {
            // Очистка токенов или сессий пользователя
            _logger.LogInformation($"User {email} logged out.");
        }

    }
}
