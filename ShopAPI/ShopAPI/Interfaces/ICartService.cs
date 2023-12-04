using ShopAPI.Models;

namespace ShopAPI.Interfaces
{
    public interface ICartService
    {
        public Task<Cart> CreateCartAsync(string name);
        public Task<CartItem> AddItemAsync(int cartId, int itemId, int quantity = 1);
        public Task RemoveItemAsync(int cartId, int itemId, int quantity = 1);
        public Task<List<CartItem>> GetCartItemsAsync(int cartId);
        public Task<Cart?> GetCartAsync(int cartId);
        public Task ClearCartAsync(int cartId);
        public Task RemoveCartAsync(int cartId);
        public Task RemoveCartItemsFromInventoryAsync(int cartId);
    }
}
