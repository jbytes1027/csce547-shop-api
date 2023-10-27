namespace ShopAPI.DTOs
{
    public class CartItemDTO : ProductDTO
    {
        public CartItemDTO(ProductDTO productDTO, int quantity)
        {
            Quantity = quantity;

            Id = productDTO.Id;
            Name = productDTO.Name;
            Category = productDTO.Category;
            Price = productDTO.Price;
            Description = productDTO.Description;
            Manufacturer = productDTO.Manufacturer;
            Details = productDTO.Details;
        }

        public int Quantity { get; set; }
    }
}
