namespace VIOBANK.API.Contracts.Account
{
    public class ChangeAccountPassword
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
