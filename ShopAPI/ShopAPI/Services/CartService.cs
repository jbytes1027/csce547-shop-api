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

    public async Task<Cart> CreateCartAsync(string name)
    {
        // Create a new cart with the given name, tracking the db changes
        var newCartEntity = _context.Carts.Add(new Cart() { Name = name });
        await _context.SaveChangesAsync();
        return newCartEntity.Entity;
    }

    /// <returns>The created Item</returns>
    public async Task<CartItem> AddItemAsync(int cartId, int productId, int quantity = 1)
    {
        AssertCartExists(cartId);

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
        AssertCartExists(cartId);

        // check if product exists
        if (_context.Products.Find(productId) is null)
        {
            throw new NotFoundException("Item not found");
        }

        CartItem? cartItem = _context.CartItems.Find(cartId, productId)
            ?? throw new NotFoundException();

        if (quantity >= cartItem.Quantity)
        {
            // Then the request is to remove all the items
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            // Then the request is to remove some of the items
            cartItem.Quantity -= quantity;
            _context.CartItems.Update(cartItem);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<CartItem>> GetCartItemsAsync(int cartId)
    {
        AssertCartExists(cartId);

        return await _context.CartItems
            .Where(item => item.CartId == cartId)
            .Include(item => item.Product)
            .ToListAsync();
    }

    /// <returns>The cart or null if not found</returns>
    public async Task<Cart?> GetCart(int cartId)
    {
        var cart = _context.Carts.Find(cartId);

        if (cart is not null)
        {
            // Populate the cart's items
            cart.Items = await GetCartItemsAsync(cartId);
        }

        return cart;
    }

    public async Task ClearCart(int cartId)
    {
        AssertCartExists(cartId);

        var cartItems = _context.CartItems.Where(c => c.CartId == cartId);

        foreach (var c in cartItems)
        {
            _context.CartItems.Remove(c);
        }

        await _context.SaveChangesAsync();
    }

    private void AssertCartExists(int cartId)
    {
        if (_context.Carts.Find(cartId) is null)
        {
            throw new NotFoundException();
        }
    }
}