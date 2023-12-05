using FU.API.Exceptions;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Interfaces;
using ShopAPI.Models;

namespace ShopAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <exception cref="NotFoundException">Thrown when an product in the cart doesn't exist</exception>
        /// <exception cref="BadRequestException">Thrown when there isn't enough stock in the inventory to remove</exception>
        public async Task RemoveCartItemsFromInventoryAsync(int cartId)
        {
            AssertCartExists(cartId);

            var cartItems = await _context.CartItems.Where(c => c.CartId == cartId).ToListAsync();

            foreach (var cartItem in cartItems)
            {
                var product = _context.Products.Find(cartItem.ProductId)
                    ?? throw new NotFoundException("Product not found");

                // Make there is enough in stock
                if (product.Stock < cartItem.Quantity)
                {
                    throw new BadRequestException("Not enough stock for " + cartItem.Product.Name);
                }

                product.Stock -= cartItem.Quantity;
                _context.Products.Update(product);
            }

            _context.SaveChanges();
        }

        /// <returns>The created cart</returns>
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

            // Check if product exists and store it for attaching to returned cartItem
            Product? product = _context.Products.Find(productId)
                ?? throw new NotFoundException("Item not found");

            // Don't add anything if not needed
            if (quantity < 0)
            {
                quantity = 0;
            }

            CartItem? cartItem = _context.CartItems.Find(cartId, productId);
            if (cartItem is not null)
            {
                // Then update the existing item
                cartItem.Quantity += quantity;

                _context.CartItems.Update(cartItem);
            }
            else
            {
                // Then create a new item
                cartItem = new()
                {
                    ProductId = productId,
                    CartId = cartId,
                    Quantity = quantity,
                };

                _context.CartItems.Add(cartItem);
            }

            if (product.Stock < cartItem.Quantity)
            {
                throw new ConflictException("Quantity in cart would be more than in stock.");
            }

            // Add the product to the returned cartItem
            cartItem.Product = product;

            await _context.SaveChangesAsync();
            return cartItem;
        }

        /// <returns>A CartItem carrying the changes that were done</returns>
        public async Task RemoveItemAsync(int cartId, int productId, int quantity = 1)
        {
            AssertCartExists(cartId);

            // Don't do anything if nothing needs removing
            if (quantity < 0)
            {
                return;
            }

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
        public async Task<Cart?> GetCartAsync(int cartId)
        {
            var cart = _context.Carts.Find(cartId);

            if (cart is not null)
            {
                // Populate the cart's items
                cart.Items = await GetCartItemsAsync(cartId);
            }

            return cart;
        }

        // Removes all the items from a cart
        public async Task ClearCartAsync(int cartId)
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
                throw new NotFoundException("Cart not found");
            }
        }

        public async Task RemoveCartAsync(int cartId)
        {
            // Clear Cart First
            await ClearCartAsync(cartId);

            // Remove Cart with Desired Id
            var carts = _context.Carts.Where(c => c.Id == cartId);
            foreach (var c in carts)
            {
                _context.Carts.Remove(c);
            }

            // Save to DB
            _context.SaveChanges();
        }
    }
}