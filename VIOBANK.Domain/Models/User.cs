namespace VIOBANK.Domain.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public Employment Employment { get; set; }
        public string IdCard { get; set; }
        public string TaxNumber { get; set; }
        public string Registration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<MobileTopup> MobileTopups { get; set; } = new List<MobileTopup>();
        public List<Contact> Contacts { get; set; } = new List<Contact>();
        public List<WithdrawnDeposit> WithdrawnDeposits { get; set; } = new List<WithdrawnDeposit>();
        public Settings Settings { get; set; }
    }
}
