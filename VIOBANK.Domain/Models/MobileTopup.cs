namespace VIOBANK.Domain.Models
{
    public class MobileTopup
    {
        public int TopupId { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
    }
}
