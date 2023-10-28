namespace ShopAPI.DTOs
{
    public class CardDTO
    {
        public int CartId { get; set; }
        public long CardNumber { get; set; }
        public string Exp { get; set; }
        public string CardHolderName { get; set; }
        public int Cvv { get; set; }
    }
}
