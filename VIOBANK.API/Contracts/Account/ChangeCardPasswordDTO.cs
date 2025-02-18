namespace VIOBANK.API.Contracts.Account
{
    public class ChangeCardPasswordDTO
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
