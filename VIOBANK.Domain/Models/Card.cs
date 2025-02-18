namespace VIOBANK.Domain.Models
{
    public class Card
    {
        public int CardId { get; set; }
        public int AccountId { get; set; }
        public string CardNumber { get; set; }
        public string HolderName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Type { get; set; }
        public string Bank { get; set; }
        public string Status { get; set; } = "Active";
        public decimal Balance { get; set; }
        public string CardPassword { get; set; }
        public int Cvc { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Account Account { get; set; }

        public List<Transaction> TransactionsFrom { get; set; } = new List<Transaction>();
        public List<Transaction> TransactionsTo { get; set; } = new List<Transaction>();
        public List<Deposit> Deposits { get; set; } = new List<Deposit>();
    }

}
