using FU.API.Exceptions;
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

    /// <returns>The created Item</returns>
    public async Task<CartItem> AddItemAsync(int cartId, int productId, int quantity = 1)
    {
        // check if cart exists
        if (_context.Carts.Find(cartId) is null)
        {
            throw new NotFoundException("Cart not found");
        }

        // check if product exists
        if (_context.Products.Find(productId) is null)
        {
            throw new NotFoundException("Item not found");
        }

        // cart item to update
        CartItem? cartItem = _context.CartItems.Find(cartId, productId);

        // If it doesn't exist, then create a new one with 0 quantity
        cartItem ??= new()
        {
            ProductId = productId,
            CartId = cartId,
            Quantity = 0,
        };

        // Add quantity to existing and update
        cartItem.Quantity += quantity;
        _context.CartItems.Update(cartItem);

        await _context.SaveChangesAsync();
        return cartItem;
    }

    /// <returns>A CartItem carrying the changes that were done</returns>
    public async Task RemoveItemAsync(int cartId, int productId, int quantity = 1)
    {
        CartItem? item = _context.CartItems.Find(cartId, productId);

        // check if cart exists
        if (item is null)
        {
            throw new NotFoundException("Cart not found");
        }

        // check if product exists
        if (_context.Products.Find(productId) is null)
        {
            throw new NotFoundException("Item not found");
        }

        if (quantity >= item.Quantity)
        {
            // Then the request is to remove all the items
            _context.CartItems.Remove(item);
        }
        else
        {
            // Then the request is to remove some of the items
            item.Quantity -= quantity;
            _context.CartItems.Update(item);
        }

        await _context.SaveChangesAsync();
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


    public Task ClearCart(int cartId)
    {
        var cartItems = _context.CartItems.Where(c => c.CartId == cartId);

        foreach (var c in cartItems)
        {
            _context.CartItems.Remove(c);
        }

        _context.SaveChanges();
        return Task.CompletedTask;
    }
}