using FluentValidation;
using VIOBANK.API.Contracts.Transaction;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequestDTO>
    {
        private static readonly string[] AllowedTransactionTypes = { "Transfer", "Payment", "Deposit", "Withdrawal" };
        private readonly ICardStore _cardStore;

        public TransactionRequestValidator(ICardStore cardStore)
        {
            _cardStore = cardStore;

            RuleFor(x => x.ToAccountCardNumber)
                .NotEmpty().WithMessage("Recipient account/card number is required.")
                .Matches(@"^\d+$").WithMessage("Account/card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ToAccountCardNumber)
                        .MustAsync(CardExists).WithMessage("Recipient card/account number does not exist.");
                });
                

            RuleFor(x => x.FromAccountCardNumber)
                .NotEmpty().WithMessage("Sender account/card number is required.")
                .Matches(@"^\d+$").WithMessage("Account/card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.FromAccountCardNumber)
                        .MustAsync(CardExists).WithMessage("Sender card/account number does not exist.");
                });
                
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Transaction type is required.")
                .Must(type => AllowedTransactionTypes.Contains(type))
                .WithMessage($"Invalid transaction type. Allowed values: {string.Join(", ", AllowedTransactionTypes)}.");

            RuleFor(x => x.Message)
                .MaximumLength(255).WithMessage("Message cannot exceed 255 characters.");

            RuleFor(x => x)
                .Must(x => x.FromAccountCardNumber != x.ToAccountCardNumber)
                .WithMessage("Sender and recipient account/card numbers must be different.");
        }

        private async Task<bool> CardExists(string cardNumber, CancellationToken cancellationToken)
        {
            return await _cardStore.GetByCardNumber(cardNumber) != null;
        }
    }
}
