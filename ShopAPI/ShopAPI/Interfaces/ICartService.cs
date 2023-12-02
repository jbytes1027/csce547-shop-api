using ShopAPI.Models;

namespace ShopAPI.Interfaces
{
    public interface ICartService
    {
        public Task<Cart> CreateCartAsync(string name);
        public Task<CartItem?> AddItemAsync(int cartId, int itemId, int quantity = 1);
        public Task<CartItem?> RemoveItemAsync(int cartId, int itemId, int quantity = 1);
        public Task<List<CartItem>> GetCartItemsAsync(int cartId);
        public Task<Cart?> GetCart(int cartId);
        public Task ClearCart(int cartId);
        public Task RemoveCart(int cartId);
    }
}
