namespace ShopAPI.DTOs
{
    // Extends productDTO by adding a quantity property
    public class CartItemDTO : ProductDTO
    {
        // Store how much of the product is in the cart
        public int Quantity { get; set; }

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
