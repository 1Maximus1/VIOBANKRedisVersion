namespace VIOBANK.API.Contracts.Transaction.ComplexDTOs
{
    public class MobileTopUpDTO
    {
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedAt { get; set; }
        public string Time { get; set; }
        public bool IsIncome { get; set; }
    }
}
