namespace VIOBANK.API.Contracts.Card
{
    public class CardsResponseDTO
    {
        public string CardNumber { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public string ExpiryDate { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Bank { get; set; }
        public int Cvc { get; set; }
    }
}
