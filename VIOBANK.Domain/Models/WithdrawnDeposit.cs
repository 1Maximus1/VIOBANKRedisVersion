
namespace VIOBANK.Domain.Models
{
    public class WithdrawnDeposit
    {
        public int WithdrawnDepositId { get; set; } 
        public int UserId { get; set; } 
        public decimal Amount { get; set; } 
        public decimal InterestEarned { get; set; } 
        public decimal TotalAmount { get; set; } 
        public string Currency { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime WithdrawnAt { get; set; }

        public User User { get; set; }
    }

}
