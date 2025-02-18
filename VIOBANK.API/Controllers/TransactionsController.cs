using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.Infrastructure;
using System.Globalization;
using VIOBANK.API.Contracts.Transaction;
using VIOBANK.API.Contracts.Transaction.ComplexDTOs;
using VIOBANK.API.Validation;
using FluentValidation;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly AccountService _accountService;
        private readonly MobileTopupService _mobileTopupService;
        private readonly DepositService _depositService;
        private readonly CardService _cardService;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IJwtProvider _jwtProvider;
        private readonly WithdrawnDepositService _withdrawnDepositService;
        private readonly IValidator<int> _userIdValidator;
        private readonly IValidator<TransactionRequestDTO> _transactionRequestValidator;

        public TransactionsController(IValidator<TransactionRequestDTO> transactionRequestValidator, ILogger<TransactionsController> logger, IJwtProvider jwtProvider, AccountService accountService, TransactionService transactionService, MobileTopupService mobileTopupService, DepositService depositService, CardService cardService, WithdrawnDepositService withdrawnDepositService, IValidator<int> userIdValidator)
        {
            _logger = logger;
            _jwtProvider = jwtProvider;
            _transactionService = transactionService;
            _accountService = accountService;
            _depositService = depositService;
            _mobileTopupService = mobileTopupService;
            _cardService = cardService;
            _withdrawnDepositService = withdrawnDepositService;
            _userIdValidator = userIdValidator;
            _transactionRequestValidator = transactionRequestValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            _logger.LogInformation("Fetching transactions");

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
                return BadRequest(new { status = "error", message = validationResultId.Errors });
            }

            var transactions = await _transactionService.GetAllTransactions(userId);
            var mobileTopUps = await _mobileTopupService.GetTopupsByUserId(userId);
            var deposits = await _depositService.GetDepositsByUserId(userId);
            var withdrawnDeposits = await _withdrawnDepositService.GetWithdrawnDepositsByUserId(userId); 

            var transactionDtos = transactions.Select(t => new TransactionResponseDTO
            {
                Amount = t.Amount,
                CurrencyTo = t.CurrencyTo,
                CurrencyFrom = t.CurrencyFrom,
                Type = t.Type,
                Description = t.Description,
                NumberFrom = t.FromCard?.CardNumber,
                NumberTo = t.ToCard?.CardNumber,
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd"),
                Time = t.CreatedAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = t.ToCard?.Account?.UserId == userId
            }).ToList();

            var mobileTopUpDtos = mobileTopUps.Select(m => new MobileTopUpDTO
            {
                Amount = m.Amount,
                PhoneNumber = m.PhoneNumber,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd"),
                Time = m.CreatedAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = false,
            }).ToList();

            var depositDtos = deposits.Select(d => new DepositDTO
            {
                Amount = d.Amount,
                Currency = d.Currency,
                CreatedAt = d.CreatedAt.ToString("yyyy-MM-dd"),
                Time = d.CreatedAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = false
            }).ToList();

            depositDtos.AddRange(withdrawnDeposits.Select(d => new DepositDTO
            {
                Amount = d.TotalAmount,
                Currency = d.Currency,
                CreatedAt = d.WithdrawnAt.ToString("yyyy-MM-dd"),
                Time = d.WithdrawnAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = true, 
            }));

            var response = new TransactionsResponseDTO
            {
                Transactions = transactionDtos,
                MobileTopUps = mobileTopUpDtos,
                Deposits = depositDtos
            };

            return Ok(response);
        }

        //Only admin
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id);
            if (transaction == null)
            {
                return NotFound(new { status = "error", message = "Transaction not found" });
            }

            var response = new TransactionResponseDTO
            {
                Amount = transaction.Amount,
                CurrencyTo = transaction.CurrencyTo,
                CurrencyFrom = transaction.CurrencyFrom,
                Type = transaction.Type,
                Description = transaction.Description,
                NumberFrom = transaction.FromCard.CardNumber,
                NumberTo = transaction.ToCard.CardNumber,
                CreatedAt = transaction.CreatedAt.ToString("yyyy-MM-dd"),
                Time = transaction.CreatedAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsIncome = transaction.ToCard?.Account?.UserId == id
            };

            return Ok(new { status = "success", transaction = response });
        }

        [HttpPost("send")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Creating new transaction");

                if (request.Amount <= 0)
                {
                    return BadRequest(new { status = "error", message = "Amount must be greater than zero" });
                }

                var token = HttpContext.Request.Cookies["tasty-cookies"];

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
                }

                var claims = _jwtProvider.GetClaimsFromToken(token);
                var userId = int.Parse(User.FindFirst("userId")?.Value);

                var validationResult = await _userIdValidator.ValidateAsync(userId);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { status = "error", message = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() });
                }

                var validationTransactionResult = await _transactionRequestValidator.ValidateAsync(request);
                if (!validationTransactionResult.IsValid)
                {
                    return BadRequest(new { status = "error", message = validationTransactionResult.Errors.Select(e => e.ErrorMessage).ToArray() });
                }

                var toCard = await _cardService.GetCardByNumber(request.ToAccountCardNumber);
                var fromCard = await _cardService.GetCardByNumber(request.FromAccountCardNumber);

                if (fromCard.Account.UserId != userId)
                {
                    return BadRequest(new { status = "error", message = "You are not allowed to send money from this card." });
                }

                var currencyTo = toCard.Account.Currency;
                var CurrencyFrom = fromCard.Account.Currency;

                var transaction = new Transaction
                {
                    FromCardId = fromCard.CardId,
                    ToCardId = toCard.CardId,
                    Amount = request.Amount,
                    Type = request.Type,
                    CurrencyFrom = CurrencyFrom,
                    CurrencyTo = currencyTo,
                    Description = request.Message,
                    CreatedAt = DateTime.UtcNow,
                    FromCard = fromCard,
                    ToCard = toCard
                };

                if (request.Type.ToLower() == "transfer")
                {
                    await _transactionService.AddTransaction(transaction);
                    try 
                    {
                       await _transactionService.Accomplish(transaction);
                    }catch(InvalidOperationException ex)
                    {
                        return BadRequest(new { status = "error", message = ex.Message });
                    }
                }
                else
                {
                    return BadRequest(new { status = "error", message = "Invalid transaction type" });
                }

                var response = new TransactionResponseDTO
                {
                    Type = "Transfer",
                    CreatedAt = transaction.CreatedAt.ToString("yyyy-MM-dd"),
                    Amount = transaction.Amount,
                    CurrencyTo = transaction.CurrencyTo,
                    CurrencyFrom = transaction.CurrencyFrom,
                    Description = transaction.Description,
                    NumberFrom = transaction.FromCard.CardNumber,
                    NumberTo = transaction.ToCard.CardNumber,
                    Time = transaction.CreatedAt.ToLocalTime().ToString("hh:mm tt", CultureInfo.InvariantCulture),
                    IsIncome = transaction.ToCard?.Account?.UserId == userId
                };
                return Ok(new { status = "success", transaction = response });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating transaction: {ex.Message}");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}

