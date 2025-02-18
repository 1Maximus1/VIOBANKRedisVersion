using FluentValidation;
using VIOBANK.API.Contracts.Card;

namespace VIOBANK.API.Validation
{
    public class CardTypeValidator : AbstractValidator<CardDTO>
    {
        private static readonly HashSet<string> AllowedCardTypes = new HashSet<string>
        {
            "Credit",
            "Debit",
            "Prepaid",
            "Virtual"
        };

        public CardTypeValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Card type is required.")
                .Must(BeAValidCardType).WithMessage($"Invalid card type. Allowed types: {string.Join(", ", AllowedCardTypes)}");
        }

        private bool BeAValidCardType(string type)
        {
            return AllowedCardTypes.Contains(type);
        }
    }
}
