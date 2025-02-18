using FluentValidation;
using VIOBANK.Domain.Stores;

namespace VIOBANK.API.Validation
{
    public class UserIdValidator : AbstractValidator<int>
    {
        private readonly IUserStore _userStore;

        public UserIdValidator(IUserStore userStore)
        {
            _userStore = userStore;

            RuleFor(userId => userId)
                .MustAsync(UserExists)
                .WithMessage("User with the given ID does not exist.");
        }

        private async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
        {
            return await _userStore.GetById(userId) != null;
        }
    }

}
