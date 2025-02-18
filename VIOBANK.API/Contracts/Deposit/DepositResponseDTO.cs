namespace VIOBANK.API.Contracts.Deposit
{
    public class DepositResponseDTO
    {
        public int DepositId { get; set; }
        public decimal Amount { get; set; }
        public decimal InitialAmount { get; set; }
        public string Currency { get; set; }
        public decimal InterestRate { get; set; }
        public int DurationMonths { get; set; }
        public bool IsActive { get; set; }
        public string DepositEndDate { get; set; }
    }
}
