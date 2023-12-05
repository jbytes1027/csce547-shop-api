using ShopAPI.Models;

namespace ShopAPI.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> SearchProductsAsync(Category? category = null, string keyword = "");
        Task<Product> CreateProductAsync(Product product, Dictionary<string, string> details);
        Task<Product?> GetProductAsync(int id);
        Task RemoveProductAsync(int id);
        Task<Product> UpdateProductPriceAsync(int id, decimal price);
        Task<Product> AddProductStockAsync(int id, int quantity);
    }
}
