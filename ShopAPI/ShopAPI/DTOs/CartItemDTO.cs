namespace ShopAPI.DTOs
{
    public class CartItemDTO : ProductDTO
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }
}
