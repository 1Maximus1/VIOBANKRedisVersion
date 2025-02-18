namespace VIOBANK.Domain.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "UAH";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
