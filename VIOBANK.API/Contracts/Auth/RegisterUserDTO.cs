using VIOBANK.API.Contracts.User;

namespace VIOBANK.API.Contracts.Auth
{
    public class RegisterUserDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public EmploymentDTO Employment { get; set; }
        public string IdCard { get; set; }
        public string TaxNumber { get; set; }
        public string Registration { get; set; }
        public string CardPassword { get; set; }
    }
}
