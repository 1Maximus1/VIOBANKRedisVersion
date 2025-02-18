namespace VIOBANK.API.Contracts.Deposit
{
    public class DepositRequestDTO
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int DurationMonths { get; set; }
        public decimal InterestRate { get; set; }
        public string CardNumber { get; set; }
    }
}
