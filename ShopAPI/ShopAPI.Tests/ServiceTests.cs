using ShopAPI.Interfaces;
using ShopAPI.Services;
using ShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Models;

namespace ShopAPI.Tests
{
    public class ServiceTests
    {
        [Fact]
        public void ProductServiceRetrieveCpu()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "shop_dev")
                          .Options;
            var context = new ApplicationDbContext(options);

            context.Set<Cpu>().Add(new Cpu()
            {
                Socket = "1151",
                Cores = 4,
                Series = "Skylake",
                IntegratedGraphics = true,
                Id = 1,
                Name = "i5-6400",
                Category = Category.Cpu,
                Price = 300,
                Manufacturer = "Intel",
                NormalizedName = "Intel i5-6400"
            });
            IProductService _productService = new ProductService(context);
            
            // Act
            var result = _productService.GetProductAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Id);
            Assert.Equal("i5-6400", result.Result.Name);
            Assert.Equal("Intel", result.Result.Manufacturer);
            Assert.Equal("Intel i5-6400", result.Result.NormalizedName);
            Assert.Equal(300, result.Result.Price);
            Assert.Equal(Category.Cpu, result.Result.Category);
        }
        
        [Fact]
        public void ProductServiceAddCpu()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "shop_dev")
                          .Options;
            var context = new ApplicationDbContext(options);
            var _cpu = new Cpu()
            {
                Id = 2,
                Name = "i5-6400",
                Category = Category.Cpu,
                Price = 300,
                Manufacturer = "Intel",
                NormalizedName = "Intel i5-6400"
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Socket","1151");
            dict.Add("Cores","4");
            dict.Add("Series","6th Gen");
            dict.Add("IntegratedGraphics","true");
            IProductService _productService = new ProductService(context);
            
            // Act
            _productService.CreateProductAsync(_cpu, dict);
            var result = _productService.GetProductAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_cpu.Id, result.Result.Id);
            Assert.Equal(_cpu.Name, result.Result.Name);
            Assert.Equal(_cpu.Manufacturer, result.Result.Manufacturer);
            Assert.Equal(_cpu.NormalizedName, result.Result.NormalizedName);
            Assert.Equal(_cpu.Price, result.Result.Price);
            Assert.Equal(_cpu.Category, result.Result.Category);
        }
        
        /* not yet implemented in service
        [Fact]
        public void ProductServiceUpdateCpu()
        {
            // Arrange
            // Act
            // Assert
        }
        */

        /*
        [Fact]
        public void ProductServiceRemoveCpu()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public void CartServiceAddItemToCart()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public void CartServiceRemoveItemFromCart()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public void CartServiceGetCart()
        {
            // Arrange
            // Act
            // Assert
        }
        */
    }
}