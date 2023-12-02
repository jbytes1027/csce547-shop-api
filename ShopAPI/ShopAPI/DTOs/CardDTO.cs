namespace ShopAPI.DTOs
{
    public class CardDTO
    {
        public int CartId { get; set; }
        public long CardNumber { get; set; }
        public string Exp { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public int Cvv { get; set; }
    }
}
