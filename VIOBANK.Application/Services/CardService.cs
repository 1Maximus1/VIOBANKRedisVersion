using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;
using VIOBANK.Infrastructure;

namespace VIOBANK.Application.Services
{
    public class CardService
    {
        private readonly ILogger<CardService> _logger;
        private readonly ICardStore _cardStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AesEncryptionService _aesEncryptionService;


        public CardService(AesEncryptionService aesEncryptionService, ILogger<CardService> logger, ICardStore cardStore, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _cardStore = cardStore;
            _passwordHasher = passwordHasher;
            _aesEncryptionService = aesEncryptionService;
        }

        public async Task<Card> GetCardByNumber(string cardNumber)
        {
            _logger.LogInformation($"Request card with number: {cardNumber}");
            return await _cardStore.GetByCardNumber(cardNumber);
        }

        public async Task<IReadOnlyList<Card>> GetCardsByUserId(int userId)
        {
            _logger.LogInformation($"Request all user cards with ID: {userId}");
            return await _cardStore.GetByUserId(userId);
        }

        public async Task<Card> GetCardById(int cardId)
        {
            _logger.LogInformation("Fetching card");
            var card = await _cardStore.GetById(cardId);
            return card;
        }

        public async Task<Card> CreateCard(Account account, string cardPassword, string holderName, DateTime ExpiryDate, string type="Debit")
        {
            _logger.LogInformation($"Creating a card: ");

            var card = new Card
            {
                AccountId = account.AccountId,
                CardNumber = GenerateCardNumber(),
                HolderName = holderName,
                ExpiryDate = DateTime.UtcNow.AddYears(5),
                Type = type,
                Bank = "viobank",
                Status = "Active",
                Balance = 0,
                CardPassword = cardPassword,
                CreatedAt = DateTime.UtcNow,
                Cvc = GenerateCVC()
            };

            return await _cardStore.Add(card);
        }

        public async Task<Card> GetCardByAccountId(int accountId)
        {
            return await _cardStore.GetByAccountId(accountId);
        }

        public async Task<Card> CreateCard(Card card)
        {
            _logger.LogInformation($"Creating a map: {card.CardNumber}");
            return await _cardStore.Add(card);
        }

        public async Task<string> GetCardPasswordByCardId(int cardId)
        {
            return await _cardStore.GetCardPasswordByCardId(cardId);
        }

        public async Task<string> GetCardPasswordByUserId(int cardId)
        {
            _logger.LogInformation("Fetching card");
            var cardPassword = await _cardStore.GetCardPasswordByUserId(cardId);
            return cardPassword;
        }

        public static string GenerateCardNumber()
        {
            Random random = new Random();

            string cardNumber = random.Next(1000, 9999).ToString() +
                                random.Next(1000, 9999).ToString() +
                                random.Next(1000, 9999).ToString() +
                                random.Next(1000, 9999).ToString();

            return cardNumber;
        }
        public int GenerateCVC()
        {
            Random random = new Random();
            return random.Next(100, 999);  
        }

        public static string GenerateCvc()
        {
            Random random = new Random();

            return random.Next(100, 999).ToString();
        }

        public async Task UpdateCard(Card card)
        {
            _logger.LogInformation($"Map update: {card.CardNumber}");
            await _cardStore.Update(card);
        }

        public async Task DeleteCard(int cardId)
        {
            _logger.LogInformation($"Removing a card with ID: {cardId}");
            await _cardStore.Delete(cardId);
        }

        public async Task<IReadOnlyList<Card>> GetAllCards()
        {
            _logger.LogInformation("Fetching all cards");
            var cards = await _cardStore.GetAll();
            return cards;
        }

        public async Task<IReadOnlyList<Card>> GetAllCardsById(int userId)
        {
            _logger.LogInformation("Fetching card");
            var cards = await _cardStore.GetByUserId(userId);
            return cards;
        }

        public async Task UpdateCardsRange(List<Card> cards)
        {
            await _cardStore.UpdateRange(cards);
        }
    }
}
