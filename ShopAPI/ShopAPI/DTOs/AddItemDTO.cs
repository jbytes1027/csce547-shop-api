namespace ShopAPI.DTOs
{
    public record AddItemDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
