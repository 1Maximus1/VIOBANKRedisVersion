using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using VIOBANK.Domain.Filters;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;
using VIOBANK.Infrastructure;

namespace VIOBANK.Application.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserStore _userStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly AesEncryptionService _aesEncryptionService; 
        private readonly AccountService _accountService;
        private readonly CardService _cardService;
        public UserService(ILogger<UserService> logger, IUserStore userStore, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, AesEncryptionService aesEncryptionService ,AccountService accountService, CardService cardService)
        {
            _logger = logger;
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _accountService = accountService;
            _cardService = cardService;
            _aesEncryptionService = aesEncryptionService;
        }

        public async Task<User> GetUserById(int userId)
        {
            _logger.LogInformation($"Getting user with ID: {userId}");
            return await _userStore.GetById(userId);
        }

        public async Task<User> GetUserByCardNumber(string cardNumber)
        {
            return await _userStore.GetByCardNumber(cardNumber);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            _logger.LogInformation($"Getting a user from Email: {email}");
            return await _userStore.GetByEmail(email);
        }

        public async Task<IReadOnlyList<User>> GetUsersByFilter(UserFilter filter)
        {
            _logger.LogInformation($"Getting a user from filter: {filter}");
            return await _userStore.GetByFilter(filter);
        }

        public async Task UpdateUser(User user)
        {
            _logger.LogInformation($"Updating user data: {user.UserId}");
            await _userStore.Update(user);
        }

        public async Task DeleteUser(int userId)
        {
            _logger.LogInformation($"Deleting a user with ID: {userId}");
            await _userStore.Delete(userId);
        }

        public async Task Register(User user, string passwordAccount, string passwordCard)
        {
            _logger.LogInformation($"Adding a new user: {user.Email}");
            
            var hashedPassword = _passwordHasher.Generate(passwordAccount);
            user.PasswordHash = hashedPassword;
            
            await _userStore.Add(user);

            var account = await _accountService.GenerateAccount(user, 0, "UAH");
            await _accountService.Add(account);

            var expiryDate = DateTime.UtcNow.AddYears(5);

           var passwordCardEncr = _aesEncryptionService.Encrypt(passwordCard);
            var card = await _cardService.CreateCard(account, passwordCardEncr, user.Surname+" "+user.Name, expiryDate);
        }

        public async Task<String> Login(string email, string password)
        {
            var user = await _userStore.GetByEmail(email);
            if(user == null)
            {
                throw new InvalidCredentialException("User not found");
            }

            var result = _passwordHasher.Verify(password, user.PasswordHash);
            if (result == false) {
                throw new InvalidCredentialException("Credentials are not correct");
            }

            var token = _jwtProvider.GenerateToken(user);
            return token;
        }

        public async Task ChangePassword(int userId, string email)
        {
            var user = await _userStore.GetByEmail(email);
            var hashedPasswordNewPassword = _passwordHasher.Generate(user.PasswordHash);
            user.PasswordHash = hashedPasswordNewPassword;
            await _userStore.Update(user);
        }

        public async Task<(bool, string)> ChangeAccountPassword(int userId, string newPassword, string oldPassword)
        {
            var user = await _userStore.GetById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with id {userId} not found.");
                return (false, "User not found.");
            }

            if (!_passwordHasher.Verify(oldPassword, user.PasswordHash))
            {
                return (false, "Incorrect old password.");
            }

            user.PasswordHash = _passwordHasher.Generate(newPassword);
            await _userStore.Update(user);
            _logger.LogInformation($"User with id {userId} updated password successfully.");

            return (true, "Account password updated successfully.");
        }

        public async Task<bool> DeleteAccount(string email)
        {
            var user = await _userStore.GetByEmail(email);
            if (user == null)
            {
                _logger.LogWarning($"User with email {email} not found.");
                return false;
            }

            await _userStore.Delete(user.UserId);
            _logger.LogInformation($"User {email} deleted.");
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> ChangeAllCardsPassword(int userId, string newPassword, string oldPassword)
        {
            var user = await GetUserById(userId);
            if (user == null)
                return (false, "User not found.");

            var encryptedPassword = await _cardService.GetCardPasswordByUserId(userId);
            if (!_aesEncryptionService.Verify(oldPassword, encryptedPassword))
                return (false, "Incorrect old password.");

            var cards = await _cardService.GetAllCardsById(userId);
            if (!cards.Any())
                return (false, "No cards found for user.");

            string hashedPassword = _aesEncryptionService.Encrypt(newPassword);
            foreach (var card in cards)
            {
                card.CardPassword = hashedPassword;
            }

            await _cardService.UpdateCardsRange(cards.ToList());

            return (true, string.Empty);
        }
    }
}
