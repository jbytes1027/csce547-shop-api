using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Interfaces;
using ShopAPI.Models;

namespace ShopAPI.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Cart> CreateCartAsync(string name)
    {
        var newCartEntity = _context.Carts.Add(new Cart() { Name = name });
        _context.SaveChanges();
        return Task.FromResult(newCartEntity.Entity);
    }

    /// <returns>The created Item or null if failed</returns>
    public Task<CartItem?> AddItemAsync(int cartId, int itemId, int quantity = 1)
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
            // Then check if the cart exists
            var cart = _context.Carts.Find(cartId);
            if (cart is null)
            {
                return Task.FromResult<CartItem?>(null);
            }

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
        return Task.FromResult<CartItem?>(item);
    }

    /// <returns>The removed item or null if failed</returns>
    public Task<CartItem?> RemoveItemAsync(int cartId, int itemId, int quantity = 1)
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
            item.Quantity -= quantity;
            _context.CartItems.Update(item);

            // Update the amount the caller is told is removed
            returnedItem.Quantity = quantity;
        }

        _context.SaveChanges();

        return Task.FromResult<CartItem?>(returnedItem);
    }

    public Task<List<CartItem>> GetCartItemsAsync(int cartId)
    {
        var cartItems = _context.CartItems
            .Where(item => item.CartId == cartId)
            .Include(item => item.Product)
            .ToList();
        return Task.FromResult(cartItems);
    }

    public async Task<Cart?> GetCart(int cartId)
    {
        var cart = _context.Carts.Find(cartId);
        if (cart is not null)
        {
            cart.Items = await GetCartItemsAsync(cartId);
        }
        return cart;
    }
}