namespace ShopAPI.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public int Quantity { get; set; } = 1;
    }
}
