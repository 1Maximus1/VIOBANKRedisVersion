namespace VIOBANK.API.Contracts.MobileTopup
{
    public class MobileTopupRequestDTO
    {
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string FromCardNumber { get; set; }
    }
}
