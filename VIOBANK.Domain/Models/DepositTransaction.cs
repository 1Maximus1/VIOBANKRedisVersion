using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIOBANK.Domain.Models
{
    public class DepositTransaction
    {
        public int TransactionId { get; set; }
        public int DepositId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Deposit Deposit { get; set; }
    }
}
