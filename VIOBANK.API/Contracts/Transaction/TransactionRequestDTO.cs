namespace VIOBANK.API.Contracts.Transaction
{
    public class TransactionRequestDTO
    {
        public string ToAccountCardNumber { get; set; } 
        public string FromAccountCardNumber { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }

}
