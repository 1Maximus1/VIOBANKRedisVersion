using FluentValidation;
using VIOBANK.API.Contracts.Deposit;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class DepositTopUpDTOValidator : AbstractValidator<DepositTopUpDTO>
    {
        private readonly ICardStore _cardStore;
        private readonly IDepositStore _depositStore;

        public DepositTopUpDTOValidator(ICardStore cardStore, IDepositStore depositStore)
        {
            _cardStore = cardStore;
            _depositStore = depositStore;

            RuleFor(x => x.DepositId)
                .GreaterThan(0).WithMessage("Deposit ID must be a positive number.")
                .MustAsync(DepositExists).WithMessage("Deposit does not exist.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches(@"^\d+$").WithMessage("Card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.CardNumber)
                         .MustAsync(CardExists).WithMessage("Card does not exist.");
                });
        }

        private async Task<bool> DepositExists(int depositId, CancellationToken cancellationToken)
        {
            return await _depositStore.GetById(depositId) != null;
        }

        private async Task<bool> CardExists(string cardNumber, CancellationToken cancellationToken)
        {
            return await _cardStore.GetByCardNumber(cardNumber) != null;
        }
    }
}
