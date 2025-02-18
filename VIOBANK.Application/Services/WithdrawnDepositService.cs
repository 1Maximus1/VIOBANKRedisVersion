using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.Application.Services
{
    public class WithdrawnDepositService
    {
        private readonly IWithdrawnDepositStore _withdrawnDepositStore;

        public WithdrawnDepositService(IWithdrawnDepositStore withdrawnDepositStore)
        {
            _withdrawnDepositStore = withdrawnDepositStore;
        }

        public async Task AddWithdrawnDeposit(WithdrawnDeposit withdrawnDeposit)
        {
            // Here you can add any additional validation or processing logic before saving
            await _withdrawnDepositStore.Add(withdrawnDeposit);
        }

        public async Task<List<WithdrawnDeposit>> GetWithdrawnDepositsByUserId(int userId)
        {
            return await _withdrawnDepositStore.GetByUserId(userId);
        }

    }
}
