using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.DTOs;
using ShopAPI.Helpers;
using ShopAPI.Interfaces;
using ShopAPI.Models;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves products based on an optional category and an optional search term.
        /// </summary>
        /// <param name="category">The optional category to filter products.</param>
        /// <param name="searchTerm">The optional search term to filter products by name or description.</param>
        /// <returns>A collection of products matching the specified criteria.</returns>
        public async Task<IEnumerable<Product>?> GetProductsAsync(Category? category, string? searchTerm)
        {
            IQueryable<Product> query = _context.Products;

            if (category != null)
            {
                query = query.Where(p => p.Category == category);
            }

            if (searchTerm != null)
            {
                // Convert searchTerm to lowercase
                var searchTermLower = searchTerm.ToLower();

                query = query.Where(p => p.NormalizedName.Contains(searchTermLower));
            }


            return await query.ToListAsync();
        }

        /// <summary>
        /// Creates a new product and adds it to the database with additional details.
        /// </summary>
        /// <param name="product">The base product to create.</param>
        /// <param name="details">Additional details to populate in the product.</param>
        /// <returns>The created product if successful, otherwise null.</returns>
        public async Task<Product?> CreateProductAsync(Product product, Dictionary<string, string> details)
        {
            if (product == null || details == null)
            {
                return null;
            }

            // Add the details to the product
            var productType = product.GetType();
            var properties = productType.GetProperties();
            foreach (var detail in details)
            {
                var property = properties.FirstOrDefault(p => p.Name.Equals(detail.Key, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    var typedValue = CommonHelper.ConvertToTypedValue(detail.Value, property.PropertyType);
                    property.SetValue(product, typedValue);
                }
            }

            product.Price = Math.Round(product.Price, 2);

            // Add the product to the database
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The retrieved product or null if not found.</returns>
        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Removes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to remove.</param>
        public async Task RemoveProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        ///  Updates a product's price.
        /// </summary>
        /// <param name="id">ID of product to update</param>
        /// <param name="price">New price value</param>
        public async Task UpdatePrice(int id, decimal price)
        {
            // Find product
            var product = await _context.Products.FindAsync(id);
            product.Price = price;

            // Update and save
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        /// Updates the stock of a product by its ID.
        /// </summary>
        /// <param name="id">Id of the product.</param>
        /// <param name="quantity">Quantity to update to.</param>
        /// <returns>Nothing.</returns>
        public async Task UpdateProductStock(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id) ?? throw new ArgumentException("Invalid product ID");

            product.Stock += quantity;

            await _context.SaveChangesAsync();

            return;
        }
    }
}
