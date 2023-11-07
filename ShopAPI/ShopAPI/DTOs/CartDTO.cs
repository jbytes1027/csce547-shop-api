namespace ShopAPI.DTOs
{
    public record CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO> Items { get; set; }
        public TotalsDTO Totals { get; set; }
        public string Name { get; set; }
    }
}