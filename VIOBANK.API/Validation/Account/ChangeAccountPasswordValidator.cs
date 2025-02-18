using FluentValidation;
using VIOBANK.API.Contracts.Account;

namespace VIOBANK.API.Validation.Account
{
    public class ChangeAccountPasswordValidator : AbstractValidator<ChangeAccountPassword>
    {
        public ChangeAccountPasswordValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Password must contain at least one special character.")
                .NotEqual(x => x.OldPassword).WithMessage("New password must be different from the old password.");
        }
    }
}
