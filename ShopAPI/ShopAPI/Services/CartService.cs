using ShopAPI.Data;
using ShopAPI.Models;

namespace ShopAPI.Services;

public class CartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<CartItem> addItemAsync(int cartId, int itemId, int quantity = 1)
    {
        CartItem? prevItem = _context.CartItems.Find(cartId, itemId);

        CartItem? item;
        if (prevItem is not null)
        {
            // Then update the existing item
            item = prevItem;
            item.Quantity += quantity;

            _context.CartItems.Update(item);
        }
        else
        {
            // Then create a new item
            item = new()
            {
                ProductId = itemId,
                CartId = cartId,
                Quantity = quantity,
            };

            _context.CartItems.Add(item);
        }

        _context.SaveChanges();
        return Task.FromResult(item);
    }

    public Task<CartItem?> removeItemAsync(int cartId, int itemId, int quantity = 1)
    {
        CartItem? item = _context.CartItems.Find(cartId, itemId);

        if (item is null)
        {
            // Nothing found to delete
            return Task.FromResult(item);
        }

        CartItem returnedItem = new()
        {
            ProductId = item.ProductId,
            CartId = item.CartId,
        };

        if (quantity >= item.Quantity)
        {
            // Then the request is to remove all the items
            _context.CartItems.Remove(item);

            // Update the amount the caller is told is removed
            returnedItem.Quantity = item.Quantity;
        }
        else
        {
            // Then the request is to remove some of the items
            _context.CartItems.Update(item);

            // Update the amount the caller is told is removed
            returnedItem.Quantity = quantity;
        }

        _context.SaveChanges();

        return Task.FromResult(returnedItem);
    }


    public Task<List<CartItem>> getItemsAsync()
    {
        var cartItems = _context.CartItems.ToList();
        return Task.FromResult(cartItems);
    }
}