using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.Infrastructure;
using System.Text.RegularExpressions;
using System.Globalization;
using VIOBANK.API.Contracts.Transaction.ComplexDTOs;
using VIOBANK.API.Contracts.MobileTopup;
using FluentValidation;
using VIOBANK.API.Validation;
using Twilio.Exceptions;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("operations")]
    public class MobileTopupController : ControllerBase
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly CardService _cardService;
        private readonly MobileTopupService _mobileTopupService;
        private readonly SmsService _smsService;
        private readonly UserService _userService;
        private readonly CurrencyExchangeService _currencyExchangeService;
        private readonly IValidator<MobileTopupRequestDTO> _validatorTopupRequest;
        private readonly IValidator<int> _userIdValidator;

        public MobileTopupController(IValidator<int> userIdValidator, IValidator<MobileTopupRequestDTO> validatorTopupRequest, IJwtProvider jwtProvider, CardService cardService, MobileTopupService mobileTopupService, SmsService smsService, UserService userService, CurrencyExchangeService currencyExchangeService)
        {
            _cardService = cardService;
            _mobileTopupService = mobileTopupService;
            _smsService = smsService;
            _jwtProvider = jwtProvider;
            _userService = userService;
            _currencyExchangeService = currencyExchangeService;
            _validatorTopupRequest = validatorTopupRequest;
            _userIdValidator = userIdValidator;
        }

        [HttpGet("mobile-topup")]
        public async Task<IActionResult> GetTopup()
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

            var mobileTopUps = await _mobileTopupService.GetTopupsByUserId(userId);

            var mobileTopUpDtos = mobileTopUps.Select(m => new MobileTopUpDTO
            {
                Amount = m.Amount,
                PhoneNumber = m.PhoneNumber,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd"),
                Time = m.CreatedAt.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = false,
            }).ToList();

            return Ok(mobileTopUpDtos);
        }

        [HttpPost("mobile-topup")]
        public async Task<IActionResult> MobileTopup([FromBody] MobileTopupRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest(new { status = "error", message = "Invalid request" });
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

            var validationResultTopupRequest = await _validatorTopupRequest.ValidateAsync(request);
            if (!validationResultTopupRequest.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultTopupRequest.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var card = await _cardService.GetCardByNumber(request.FromCardNumber);

            if (card.Account == null || card.Account.User == null)
            {
                return BadRequest(new { status = "error", message = "Invalid card owner" });
            }

            if (card.Account.UserId != userId)
            {
                return Forbid("You are not allowed to send money from this card.");
            }

            string cardCurrency = card.Account.Currency;
            decimal amountToDeduct = request.Amount; 

            if (card.Balance < amountToDeduct)
            {
                return BadRequest(new { status = "error", message = "Insufficient funds" });
            }
         
            if (cardCurrency != "UAH")
            {
                try
                {
                    var exchangeRate = await _currencyExchangeService.GetExchangeRateAsync(cardCurrency, "UAH");
                    if (exchangeRate <= 0)
                    {
                        return StatusCode(500, new { status = "error", message = "Failed to retrieve exchange rate" });
                    }

                    amountToDeduct = request.Amount * exchangeRate;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { status = "error", message = "Currency exchange service error", details = ex.Message });
                }
            }            

            card.Balance -= request.Amount;
            await _cardService.UpdateCard(card);

            var topup = new MobileTopup
            {
                PhoneNumber = request.PhoneNumber,
                Amount = amountToDeduct,
                User = card.Account.User,
                CreatedAt = DateTime.UtcNow
            };
            await _mobileTopupService.AddTopup(topup);

            try 
            {
                _smsService.SendSms(request.PhoneNumber,
                    $"Your mobile account has been replenished with {amountToDeduct} UAH. ");
            }catch(ApiException ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }

            return Ok(new
            {
                status = "success",
                message = "Mobile top-up successful"
            });
        }
    }
}
