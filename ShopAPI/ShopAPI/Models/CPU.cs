namespace ShopAPI.Models
{
    public class CPU
    {
        public int Id { get; set; }
        public int Cores { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
