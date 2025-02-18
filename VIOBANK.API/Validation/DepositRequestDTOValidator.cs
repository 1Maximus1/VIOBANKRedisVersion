using FluentValidation;
using VIOBANK.API.Contracts.Deposit;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class DepositRequestDTOValidator : AbstractValidator<DepositRequestDTO>
    {
        private readonly ICardStore _cardStore;
        private readonly CurrencyExchangeService _exchangeService;

        public DepositRequestDTOValidator(ICardStore cardStore, CurrencyExchangeService exchangeService)
        {
            _cardStore = cardStore;
            _exchangeService = exchangeService;

            RuleFor(x => x.Amount)
                .MustAsync((dto, amount, cancellationToken) => BeValidAmount(amount, dto.CardNumber, cancellationToken)).WithMessage("Amount must be greater than 1000.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Must(BeValidCurrency).WithMessage("Currency must be USD, EUR, or UAH.");

            RuleFor(x => x.DurationMonths)
                .GreaterThanOrEqualTo(4).WithMessage("Duration must be greater than 3 months.");

            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(4).WithMessage("Interest rate must be 0 or greater.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches("^\\d+$").WithMessage("Card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.CardNumber)
                        .MustAsync(CardExists).WithMessage("Card does not exist.");
                });
                
        }

        private bool BeValidCurrency(string currency)
        {
            return currency == "USD" || currency == "EUR" || currency == "UAH";
        }

        private async Task<bool> CardExists(string cardNumber, CancellationToken cancellationToken)
        {
            return await _cardStore.GetByCardNumber(cardNumber) != null;
        }

        private async Task<bool> BeValidAmount(decimal amount, string fromCardNumber, CancellationToken cancellationToken)
        {
            var card = await _cardStore.GetByCardNumber(fromCardNumber);
            if (card == null) return false;

            if (card.Account.Currency == "UAH")
            {
                return amount > 1000;
            }

            try
            {
                decimal rate = await _exchangeService.GetExchangeRateAsync(card.Account.Currency, "UAH");
                decimal convertedAmount = amount * rate;
                return convertedAmount > 1000;
            }
            catch
            {
                return false;
            }
        }
    }
}