using ShopAPI.Models;

namespace ShopAPI.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public Dictionary<string, string> Details { get; set; }
    }
}
