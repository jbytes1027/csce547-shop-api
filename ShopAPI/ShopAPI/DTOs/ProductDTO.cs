namespace ShopAPI.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public Dictionary<string, string> Details { get; set; } = new();
        public int Stock { get; set; }
    }
}
