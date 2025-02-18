using FluentValidation;
using VIOBANK.API.Contracts.MobileTopup;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class MobileTopupRequestValidator : AbstractValidator<MobileTopupRequestDTO>
    {
        private readonly ICardStore _cardStore;
        private readonly CurrencyExchangeService _exchangeService;

        public MobileTopupRequestValidator(ICardStore cardStore, CurrencyExchangeService exchangeService)
        {
            _cardStore = cardStore;
            _exchangeService = exchangeService;

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Amount)
                .GreaterThan(10).WithMessage("Amount must be greater than 10 UAH.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Amount)
                        .MustAsync((dto, amount, cancellationToken) => BeValidAmount(amount, dto.FromCardNumber, cancellationToken)).WithMessage("Converted amount must be greater than 10 UAH.");
                });

            RuleFor(x => x.FromCardNumber)
                .NotEmpty().WithMessage("Sender card number is required.")
                .Matches(@"^\d+$").WithMessage("Card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.FromCardNumber)
                         .MustAsync(CardExists).WithMessage("Sender card does not exist.");
                });
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
                return amount > 40;
            }

            try
            {
                decimal rate = await _exchangeService.GetExchangeRateAsync(card.Account.Currency, "UAH");
                decimal convertedAmount = amount * rate;
                return convertedAmount > 40;
            }
            catch
            {
                return false;
            }
        }
    }

}
