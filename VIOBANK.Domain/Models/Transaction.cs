namespace VIOBANK.Domain.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int FromCardId { get; set; }
        public int ToCardId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CurrencyFrom { get; set; } = string.Empty;
        public string CurrencyTo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Card FromCard { get; set; }
        public Card ToCard { get; set; }
    }
}
