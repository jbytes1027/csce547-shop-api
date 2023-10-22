using ShopAPI.DTOs;
using ShopAPI.Models;

namespace ShopAPI.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>?> GetProdcutsAsync(Category? category, string? searchString);
        Task<Product?> CreateProductAsync(Product product, Dictionary<string, string> details);
        Task<Product?> GetProductAsync(int id);
        Task RemoveProductAsync(int id);
    }
}
