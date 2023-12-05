using FU.API.Exceptions;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Helpers;
using ShopAPI.Interfaces;
using ShopAPI.Models;

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
        /// <param name="keyword">The optional search term to filter products by name or description.</param>
        /// <returns>A collection of products matching the specified criteria.</returns>
        public async Task<IEnumerable<Product>> SearchProductsAsync(Category? category = null, string keyword = "")
        {
            IQueryable<Product> query = _context.Products;

            if (category != null)
            {
                query = query.Where(p => p.Category == category);
            }

            // Convert searchTerm to lowercase to match case of NormalizedName
            var keywordNormalized = keyword.ToLower();
            query = query.Where(p => p.NormalizedName.Contains(keywordNormalized));

            return await query.ToListAsync();
        }

        /// <summary>
        /// Creates a new product and adds it to the database with additional details.
        /// </summary>
        /// <param name="product">The base product to create.</param>
        /// <param name="details">Additional details to populate in the product.</param>
        /// <returns>The created product.</returns>
        public async Task<Product> CreateProductAsync(Product product, Dictionary<string, string> details)
        {
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
            var product = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///  Updates a product's price.
        /// </summary>
        /// <param name="id">ID of product to update</param>
        /// <param name="price">New price value</param>
        public async Task<Product> UpdateProductPriceAsync(int id, decimal price)
        {
            // Find product
            var product = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException("Product not found");

            product.Price = price;

            // Update and save
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }

        /// Updates the stock of a product by its ID.
        /// </summary>
        /// <param name="id">Id of the product.</param>
        /// <param name="quantity">Quantity to update to.</param>
        /// <returns>Nothing.</returns>
        public async Task<Product> AddProductStockAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new ArgumentException("Invalid product ID");

            product.Stock += quantity;

            await _context.SaveChangesAsync();

            return product;
        }
    }
}
