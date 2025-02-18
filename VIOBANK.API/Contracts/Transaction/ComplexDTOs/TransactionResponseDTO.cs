namespace VIOBANK.API.Contracts.Transaction.ComplexDTOs
{
    public class TransactionResponseDTO
    {
        public decimal Amount { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string NumberFrom { get; set; }
        public string? NumberTo { get; set; }
        public string CreatedAt { get; set; }
        public string Time { get; set; }
        public bool IsIncome { get; set; }
    }
}