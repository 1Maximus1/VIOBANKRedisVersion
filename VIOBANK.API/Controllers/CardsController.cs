using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.API.Contracts.Card;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.Infrastructure;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("cards")]
    public class CardsController : Controller
    {
        private readonly CardService _cardService;
        private readonly IJwtProvider _jwtProvider;
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        private readonly IValidator<int> _userIdValidator;
        private readonly IValidator<CardDTO> _cardTypeValidator;
        private readonly AesEncryptionService _aesEncryptionService;

        public CardsController(AesEncryptionService aesEncryptionService, IValidator<CardDTO> cardTypeValidator, IValidator<int> userIdValidator, CardService cardService, IJwtProvider jwtProvider, UserService userService, AccountService accountService)
        {
            _cardService = cardService;
            _jwtProvider = jwtProvider;
            _userService = userService;
            _accountService = accountService;
            _userIdValidator = userIdValidator;
            _cardTypeValidator = cardTypeValidator;
            _aesEncryptionService = aesEncryptionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);
            
            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var cards = await _cardService.GetAllCardsById(userId);

            var cardDTOs = await Task.WhenAll(cards.Select(async card => new CardsResponseDTO
            {
                CardNumber = card.CardNumber,
                HolderName = card.HolderName,
                Balance = card.Balance,
                ExpiryDate = card.ExpiryDate.ToString("yyyy-MM-dd"),
                Type = card.Type,
                Status = card.Status,
                Bank = card.Bank,
                Currency = (await _accountService.GetAccountByCardNumber(card.CardNumber)).Currency,
                Cvc = card.Cvc
            }));

            return Ok(cardDTOs);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCard([FromBody] CardDTO cardDTO)
        {
            var currencies = new List<string> { "EUR", "USD", "UAH" };

            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultCardType = await _cardTypeValidator.ValidateAsync(cardDTO);
            if (!validationResultCardType.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultCardType.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var user = await _userService.GetUserById(userId);

            var cardPasswordEncr = await _cardService.GetCardPasswordByUserId(userId);

            Account account; 
            if (currencies.Contains(cardDTO.Currency))
            {
                account = await _accountService.GetAccountByType(user.UserId, cardDTO.Currency);
                if(account == null)
                {
                    account = await _accountService.GenerateAccount(user, 0, cardDTO.Currency);

                    await _accountService.CreateAccount(account);
                }
            }
            else
            {
                return NotFound(new { status = "error", message = "Currency not found" });
            }

            var expiryDate = DateTime.UtcNow.AddYears(5);
            var newCard = await _cardService.CreateCard(account, cardPasswordEncr, user.Surname + " " + user.Name, expiryDate, cardDTO.Type);

            return Ok(new { status = "success", message = "Card added successfully"});
        }
    }
}
