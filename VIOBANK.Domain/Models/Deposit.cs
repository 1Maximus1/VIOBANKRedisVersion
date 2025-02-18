namespace VIOBANK.Domain.Models
{
    public class Deposit
    {
        public int DepositId { get; set; } 
        public int CardId { get; set; } 
        public decimal Amount { get; set; }
        public decimal InitialAmount { get; set; }
        public string Currency { get; set; }
        public int DurationMonths { get; set; }
        public decimal InterestRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Card Card { get; set; }
        public List<DepositTransaction> DepositTransactions { get; set; }
    }


}
