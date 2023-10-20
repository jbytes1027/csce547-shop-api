namespace ShopAPI.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Color { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
