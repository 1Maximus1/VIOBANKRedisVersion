using FluentValidation;
using VIOBANK.API.Contracts.Contact;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class ContactRequestDTOValidator : AbstractValidator<ContactRequestDTO>
    {
        private readonly ICardStore _cardStore;
        private readonly IContactStore _contactStore;

        public ContactRequestDTOValidator(ICardStore cardStore, IContactStore contactStore)
        {
            _cardStore = cardStore;
            _contactStore = contactStore;

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches(@"^\d+$").WithMessage("Card number must contain only digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.CardNumber)
                        .MustAsync(CardExists).WithMessage("Card does not exist.")
                        .MustAsync(ContactNotExists).WithMessage("This card is already in contacts.");
                });
        }

        private async Task<bool> CardExists(string cardNumber, CancellationToken cancellationToken)
        {
            return await _cardStore.GetByCardNumber(cardNumber) != null;
        }

        private async Task<bool> ContactNotExists(string cardNumber, CancellationToken cancellationToken)
        {
            return !(await _contactStore.ContactExists(cardNumber));
        }
    }
}
