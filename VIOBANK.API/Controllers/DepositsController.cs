using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.API.Contracts.Deposit;
using VIOBANK.API.Validation;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.Infrastructure;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("deposits")]
    public class DepositsController : Controller
    {
        private readonly DepositService _depositService;
        private readonly ILogger<DepositsController> _logger;
        private readonly CardService _cardService;
        private readonly CurrencyExchangeService _currencyExchangeService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IValidator<DepositRequestDTO> _validatorDepositRequest;
        private readonly IValidator<DepositTopUpDTO> _validatorDepositTopUp;
        private readonly IValidator<int> _userIdValidator;
        public DepositsController(IValidator<DepositTopUpDTO> validatorDepositTopUp, IValidator<DepositRequestDTO> validatorDepositRequest, IValidator<int> userIdValidator, DepositService depositService, ILogger<DepositsController> logger, CardService cardService, CurrencyExchangeService currencyExchangeService, IJwtProvider jwtProvider)
        {
            _depositService = depositService;
            _logger = logger;
            _cardService = cardService;
            _currencyExchangeService = currencyExchangeService;
            _jwtProvider = jwtProvider;
            _validatorDepositRequest = validatorDepositRequest;
            _userIdValidator = userIdValidator;
            _validatorDepositTopUp = validatorDepositTopUp;
        }

        [HttpPost("open")]
        public async Task<IActionResult> OpenDeposit([FromBody] DepositRequestDTO depositRequestDTO)
        {
            if (depositRequestDTO == null)
            {
                return BadRequest(new { status = "error", message = "Invalid deposit request." });
            }

            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultDepositRequest = await _validatorDepositRequest.ValidateAsync(depositRequestDTO);
            if (!validationResultDepositRequest.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultDepositRequest.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            try
            {
                var card = await _cardService.GetCardByNumber(depositRequestDTO.CardNumber);
                if (card == null)
                {
                    return BadRequest(new { status = "error", message = "Card not found." });
                }
                if (card.Account.UserId != userId)
                {
                    return Forbid("You are not allowed to send money from this card.");
                }


                decimal amountToDeduct = depositRequestDTO.Amount;

                if (!card.Account.Currency.Equals(depositRequestDTO.Currency, StringComparison.OrdinalIgnoreCase))
                {
                    var exchangeRate = await _currencyExchangeService.GetExchangeRateAsync(card.Account.Currency, depositRequestDTO.Currency);
                    amountToDeduct = depositRequestDTO.Amount / exchangeRate;
                }

                if (card.Balance < amountToDeduct)
                {
                    return BadRequest(new { status = "error", message = "Insufficient funds on card." });
                }

                card.Balance -= amountToDeduct;
                await _cardService.UpdateCard(card);

                var deposit = new Deposit
                {
                    CardId = card.CardId,
                    Amount = depositRequestDTO.Amount,
                    InitialAmount = depositRequestDTO.Amount,
                    Currency = depositRequestDTO.Currency,
                    DurationMonths = depositRequestDTO.DurationMonths,
                    InterestRate = depositRequestDTO.InterestRate,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    IsActive = true
                };

                await _depositService.AddDeposit(deposit);
                return Ok(new { status = "success", message = "Deposit successfully created."});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding deposit: {ex.Message}");
                return StatusCode(500, new { status = "error", message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeposits()
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var deposits = await _depositService.GetDepositsByUserId(userId);

            var depositsDTOs = deposits.Select(d => new DepositResponseDTO
            {
                DepositId = d.DepositId,
                Amount = d.Amount,
                InitialAmount = d.InitialAmount,
                Currency = d.Currency,
                InterestRate = d.InterestRate,
                DurationMonths = d.DurationMonths,
                IsActive = d.IsActive,
                DepositEndDate = d.CreatedAt.AddMonths(d.DurationMonths).ToString("yyyy-MM-dd")
            }).ToList();

            return Ok(depositsDTOs);
        }

        [HttpPost("topup")]
        public async Task<IActionResult> TopUpDeposit([FromBody] DepositTopUpDTO depositTopUpDTO)
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultDepositTopup = await _validatorDepositTopUp.ValidateAsync(depositTopUpDTO);
            if (!validationResultDepositTopup.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultDepositTopup.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var deposit = await _depositService.GetDepositById(depositTopUpDTO.DepositId);

            if (!deposit.IsActive)
            {
                return BadRequest(new { status = "error", message = "Deposit is inactive and cannot be topped up." });
            }
            if (deposit.Card.Account.UserId != userId)
            {
                return Forbid("You are not allowed to top up this deposit.");
            }

            var depositEndDate = deposit.CreatedAt.AddMonths(deposit.DurationMonths);
            if (DateTime.UtcNow >= depositEndDate)
            {
                return BadRequest(new { status = "error", message = "Deposit term has ended and cannot be topped up." });
            }


            var card = await _cardService.GetCardByNumber(depositTopUpDTO.CardNumber);
            if (card == null)
            {
                return NotFound(new { status = "error", message = "Card not found." });
            }

            if (card.Account.UserId != userId)
            {
                return Forbid("You are not allowed to use this card.");
            }

            decimal amountToDeduct = depositTopUpDTO.Amount;

            if (card.Balance < amountToDeduct)
            {
                return BadRequest(new { status = "error", message = "Insufficient funds on the card." });
            }

            if (!card.Account.Currency.Equals(deposit.Currency, StringComparison.OrdinalIgnoreCase))
            {
                var exchangeRate = await _currencyExchangeService.GetExchangeRateAsync(card.Account.Currency, deposit.Currency);
                amountToDeduct = depositTopUpDTO.Amount * exchangeRate;
            }

            
            var result = await _depositService.TopUpDeposit(depositTopUpDTO.DepositId, amountToDeduct);

            card.Balance -= amountToDeduct;
            await _cardService.UpdateCard(card);
            

            if (!result.Success)
            {
                return BadRequest(new { status = "error", message = "Deposit top-up failed." });
            }

            return Ok(new { status = "success", message = "Deposit topped up successfully." });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawDeposit([FromBody] WithdrawDepositDTO withdrawRequest)
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { status = "error", message = "Unauthorized: Token is missing in cookies." });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var result = await _depositService.WithdrawDeposit(withdrawRequest.DepositId, userId);


            if (!result.Success)
            {
                return BadRequest(new { status = "error", message = result.Message });
            }

            return Ok(new { status = "success", message = "Deposit successfully withdrawn.", amountPaid = result.AmountPaid });
        }
    }
}
