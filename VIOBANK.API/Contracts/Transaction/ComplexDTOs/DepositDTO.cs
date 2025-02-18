namespace VIOBANK.API.Contracts.Transaction.ComplexDTOs
{
    public class DepositDTO
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CreatedAt { get; set; }
        public string Time { get; set; }
        public bool IsIncome { get; set; }
    }
}
