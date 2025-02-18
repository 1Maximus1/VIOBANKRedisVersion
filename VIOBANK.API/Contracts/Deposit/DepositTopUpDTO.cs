namespace VIOBANK.API.Contracts.Deposit
{
    public class DepositTopUpDTO
    {
        public int DepositId { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
    }
}
