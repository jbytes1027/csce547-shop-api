namespace ShopAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CartItem> Items = new();
    }
}