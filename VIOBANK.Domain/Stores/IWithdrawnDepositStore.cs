using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Domain.Stores
{
    public interface IWithdrawnDepositStore
    {
        Task Add(WithdrawnDeposit withdrawnDeposit);
        Task<List<WithdrawnDeposit>> GetByUserId(int userId);
    }
}
