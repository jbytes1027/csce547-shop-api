using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.DTOs;
using ShopAPI.Helpers;
using ShopAPI.Interfaces;
using ShopAPI.Models;
using System.Reflection;

namespace ShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>?> GetProdcutsAsync(Category? category, string? searchTerm)
        {
            IQueryable<Product> query = _context.Products;

            if (category != null)
            {
                query = query.Where(p => p.Category == category);
            }

            if (searchTerm != null)
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

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

            // Add the product to the database
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task RemoveProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
