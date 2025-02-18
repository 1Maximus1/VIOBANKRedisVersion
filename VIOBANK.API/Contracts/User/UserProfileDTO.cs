namespace VIOBANK.API.Contracts.User
{
    public class UserProfileDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public EmploymentDTO Employment { get; set; }

        public string IdCard { get; set; }
        public string TaxNumber { get; set; }
        public string Registration { get; set; }
    }
}
