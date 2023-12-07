namespace ShopAPI.DTOs
{
    // Extends productDTO by adding a quantity property
    public class CartItemDTO
    {
        // Store how much of the product is in the cart
        public int Quantity { get; set; }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public Dictionary<string, string> Details { get; set; } = new();

        public CartItemDTO(ProductDTO productDTO, int quantity)
        {
            Quantity = quantity;

            Id = productDTO.Id;
            Name = productDTO.Name;
            Category = productDTO.Category;
            Price = productDTO.Price;
            Manufacturer = productDTO.Manufacturer;
            Details = productDTO.Details;
        }
    }
}
