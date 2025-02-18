using FluentValidation;
using VIOBANK.API.Contracts.User;
using VIOBANK.Domain.Filters;
using VIOBANK.Domain.Stores;


namespace VIOBANK.Application.Validation
{
    public class UserProfileDTOValidator : AbstractValidator<UserProfileDTO>
    {
        private readonly IUserStore _userStore;

        public UserProfileDTOValidator(IUserStore userStore)
        {
            _userStore = userStore;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be minimum 2 characters.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z'-]+$").WithMessage("Name can only contain letters, apostrophes, and hyphens. Spaces are not allowed.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MinimumLength(2).WithMessage("Surname must be minimum 2 characters.")
                .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z'-]+$").WithMessage("Surname can only contain letters, apostrophes, and hyphens. Spaces are not allowed.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid email format. Email must contain '@' and a valid domain.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .MustAsync(IsEmailUnique).WithMessage("Email is already in use.");
                });
               

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Phone)
                        .MustAsync(IsPhoneUnique).WithMessage("Phone number is already in use.");
                });
               

            RuleFor(x => x.IdCard)
                .NotEmpty().WithMessage("ID Card is required.")
                .Matches(@"^\d+$").WithMessage("ID Card must contain only digits.")
                .MaximumLength(20).WithMessage("ID Card cannot exceed 20 characters.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.IdCard)
                        .MustAsync(IsIdCardUnique).WithMessage("ID Card is already in use.");
                });
                

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Tax Number is required.")
                .Matches(@"^\d+$").WithMessage("Tax Number must contain only digits.")
                .Length(10).WithMessage("Tax Number must be exactly 10 digits.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.TaxNumber)
                        .MustAsync(IsTaxNumberUnique).WithMessage("Tax Number is already in use.");
                });
               

            RuleFor(x => x.Registration)
                .NotEmpty().WithMessage("Registration is required.")
                .MaximumLength(50).WithMessage("Registration cannot exceed 50 characters.");

            RuleFor(x => x.Employment)
                .NotNull().WithMessage("Employment information is required.")
                .SetValidator(new EmploymentDTOValidator());
        }
        private async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
        {
            return await _userStore.GetByEmail(email) == null;
        }

        private async Task<bool> IsPhoneUnique(string phone, CancellationToken cancellationToken)
        {
            var user = await _userStore.GetByFilter(new UserFilter { Phone = phone });
            return !user.Any();
        }

        private async Task<bool> IsIdCardUnique(string idCard, CancellationToken cancellationToken)
        {
            var user = await _userStore.GetByFilter(new UserFilter { IdCard = idCard });
            return !user.Any();
        }

        private async Task<bool> IsTaxNumberUnique(string taxNumber, CancellationToken cancellationToken)
        {
            var user = await _userStore.GetByFilter(new UserFilter { TaxNumber = taxNumber });
            return !user.Any();
        }

    }

    public class EmploymentDTOValidator : AbstractValidator<EmploymentDTO>
    {
        public EmploymentDTOValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(20).WithMessage("Type cannot exceed 20 characters.");

            RuleFor(x => x.Income)
                .NotEmpty().WithMessage("Income is required.")
                .Matches(@"^\d+(\.\d{1,2})?\s?(USD|EUR|UAH)$")
                .WithMessage("Invalid income format. Example: '5000 USD'.");
        }
    }
}