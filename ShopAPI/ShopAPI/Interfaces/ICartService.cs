using ShopAPI.Models;

namespace ShopAPI.Interfaces
{
    public interface ICartService
    {

        Task<CartItem> AddItemAsync(int cartId, int itemId, int quantity = 1);
        Task<CartItem?> RemoveItemAsync(int cartId, int itemId, int quantity = 1);
        Task<List<CartItem>> GetAllItemsAsync();
        Task<List<CartItem>> GetCartItemsAsync(int cartId);
    }
}
