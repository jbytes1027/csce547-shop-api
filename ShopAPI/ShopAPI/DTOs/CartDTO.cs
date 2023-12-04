namespace ShopAPI.DTOs
{
    public record CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
        public TotalsDTO Totals { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }
}