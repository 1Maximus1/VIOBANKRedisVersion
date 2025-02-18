namespace VIOBANK.API.Contracts.Transaction.ComplexDTOs
{
    public class TransactionsResponseDTO
    {
        public List<TransactionResponseDTO> Transactions { get; set; }
        public List<MobileTopUpDTO> MobileTopUps { get; set; }
        public List<DepositDTO> Deposits { get; set; }
    }
}
