using FluentValidation;
using VIOBANK.API.Contracts.Account;

namespace VIOBANK.API.Validation.Account
{
    public class ChangeCardPasswordDTOValidator : AbstractValidator<ChangeCardPasswordDTO>
    {
        public ChangeCardPasswordDTOValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.")
                .Matches(@"^\d{4}$").WithMessage("Old password must be exactly 4 digits.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Matches(@"^\d{4}$").WithMessage("New password must be exactly 4 digits.")
                .NotEqual(x => x.OldPassword).WithMessage("New password cannot be the same as the old password.");
        }
    }
}
