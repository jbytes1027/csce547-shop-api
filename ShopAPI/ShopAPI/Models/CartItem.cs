using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAPI.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
