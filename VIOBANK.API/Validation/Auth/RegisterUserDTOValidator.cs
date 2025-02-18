using FluentValidation;
using VIOBANK.API.Contracts.Auth;
using VIOBANK.Application.Validation;
using VIOBANK.Domain.Filters;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation.Auth
{
    public class RegisterUserDTOValidator : AbstractValidator<RegisterUserDTO>
    {
        private readonly IUserStore _userStore;

        public RegisterUserDTOValidator(IUserStore userStore)
        {
            _userStore = userStore;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z'-]+$").WithMessage("Name can only contain letters, apostrophes, and hyphens.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MinimumLength(2).WithMessage("Surname must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z'-]+$").WithMessage("Surname can only contain letters, apostrophes, and hyphens.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .MustAsync(BeUniqueEmail).WithMessage("Email is already in use.");
                });
                
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@".*\d.*").WithMessage("Password must contain at least one digit.")
                .Matches(@".*[A-Z].*").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Phone)
                        .MustAsync(BeUniquePhone).WithMessage("Phone number is already in use.");
                });
               
            RuleFor(x => x.IdCard)
                .NotEmpty().WithMessage("ID Card is required.")
                .Matches(@"^\d+$").WithMessage("ID Card must contain only digits.")
                .MaximumLength(20).WithMessage("ID Card cannot exceed 20 characters.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.IdCard)
                        .MustAsync(BeUniqueIdCard).WithMessage("ID Card is already in use.");
                });
                
            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Tax Number is required.")
                .Matches(@"^\d+$").WithMessage("Tax Number must contain only digits.")
                .Length(10).WithMessage("Tax Number must be exactly 10 digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.TaxNumber)
                        .MustAsync(BeUniqueTaxNumber).WithMessage("Tax Number is already in use.");
                });

            RuleFor(x => x.Registration)
                .NotEmpty().WithMessage("Registration is required.")
                .MaximumLength(50).WithMessage("Registration cannot exceed 50 characters.");

            RuleFor(x => x.CardPassword)
                .NotEmpty().WithMessage("Card password is required.")
                .Length(4).WithMessage("Card password must be 4 characters long.")
                .Matches(@"^\d+$").WithMessage("Card password must contain only digits.");

            RuleFor(x => x.Employment)
                .NotNull().WithMessage("Employment information is required.")
                .SetValidator(new EmploymentDTOValidator());
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return await _userStore.GetByEmail(email) == null;
        }

        private async Task<bool> BeUniquePhone(string phone, CancellationToken cancellationToken)
        {
            var users = await _userStore.GetByFilter(new UserFilter { Phone = phone });
            return !users.Any(); 
        }

        private async Task<bool> BeUniqueIdCard(string idCard, CancellationToken cancellationToken)
        {
            var users = await _userStore.GetByFilter(new UserFilter { IdCard = idCard });
            return !users.Any();
        }

        private async Task<bool> BeUniqueTaxNumber(string taxNumber, CancellationToken cancellationToken)
        {
            var users = await _userStore.GetByFilter(new UserFilter { TaxNumber = taxNumber });
            return !users.Any();
        }
    }
}
