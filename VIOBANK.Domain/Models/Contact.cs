namespace VIOBANK.Domain.Models
{
    public class Contact
    {
        public int ContactId { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCard { get; set; }

        public User User { get; set; }
    }
}
