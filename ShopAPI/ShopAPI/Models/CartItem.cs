namespace ShopAPI.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
