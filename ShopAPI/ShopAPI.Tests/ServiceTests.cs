using ShopAPI.Interfaces;
using ShopAPI.Services;
using ShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Models;
using Xunit.Abstractions;

namespace ShopAPI.Tests
{
    public class ServiceTests
    {
        // Use in place of Console.WriteLine for logging
        private readonly ITestOutputHelper output;

        public ServiceTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void ProductServiceRetrieveCpu()
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
            IProductService productService = new ProductService(context);

            // Act
            var result = await productService.GetProductAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("i5-6400", result.Name);
            Assert.Equal("Intel", result.Manufacturer);
            Assert.Equal("Intel i5-6400", result.NormalizedName);
            Assert.Equal(300, result.Price);
            Assert.Equal(Category.Cpu, result.Category);
        }
        
        [Fact]
        public async void ProductServiceAddCpu()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "shop_dev")
                          .Options;
            var context = new ApplicationDbContext(options);
            var cpu = new Cpu()
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
            IProductService productService = new ProductService(context);

            // Act
            await productService.CreateProductAsync(cpu, dict);
            var result = await productService.GetProductAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cpu.Id, result.Id);
            Assert.Equal(cpu.Name, result.Name);
            Assert.Equal(cpu.Manufacturer, result.Manufacturer);
            Assert.Equal(cpu.NormalizedName, result.NormalizedName);
            Assert.Equal(cpu.Price, result.Price);
            Assert.Equal(cpu.Category, result.Category);
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

        [Fact]
        public async void ProductServiceRemoveCpu()
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
            IProductService productService = new ProductService(context);

            // Act
            await productService.RemoveProductAsync(1);
            var result = await productService.GetProductAsync(1);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async void CartServiceAddItemToCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "shop_dev")
                          .Options;
            var context = new ApplicationDbContext(options);
            
            Cpu cpu = new Cpu()
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
            };
            context.Set<Cpu>().Add(cpu);

            ICartService cartService = new CartService(context);

            // Act
            // Defaults to quantity of 1
            await cartService.AddItemAsync(1, 1);
            var cartList = cartService.GetCartItemsAsync(1);
            var result = cartList.Result[0];

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CartId);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(1, result.Quantity);
            Assert.Equal(cpu.Id,result.Product.Id);
            Assert.Equal(cpu.Name, result.Product.Name);
            Assert.Equal(cpu.Manufacturer, result.Product.Manufacturer);
            Assert.Equal(cpu.NormalizedName, result.Product.NormalizedName);
            Assert.Equal(cpu.Price, result.Product.Price);
            Assert.Equal(cpu.Category, result.Product.Category);
        }

        [Fact]
        public async void CartServiceRemoveItemFromCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                              .UseInMemoryDatabase(databaseName: "shop_dev")
                              .Options;
            var context = new ApplicationDbContext(options);

            Cpu cpu = new Cpu()
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
            };
            context.Set<Cpu>().Add(cpu);
            ICartService cartService = new CartService(context);

            // Act
            /* Case: Removing 1 from quantity of 1
            {
                await cartService.AddItemAsync(1, 1, 1);
                await cartService.RemoveItemAsync(1, 1, 1);
                var cartList = cartService.GetCartItemsAsync(1);
                var result = cartList.Result.Count;
                Assert.Equal(0, result);
            }
            */
            await cartService.AddItemAsync(1, 1, 3);
            await cartService.RemoveItemAsync(1, 1, 1);
            var cartList = await cartService.GetCartItemsAsync(1);
            var result = cartList[0];

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CartId);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(cpu.Id,result.Product.Id);
            Assert.Equal(cpu.Name, result.Product.Name);
            Assert.Equal(cpu.Manufacturer, result.Product.Manufacturer);
            Assert.Equal(cpu.NormalizedName, result.Product.NormalizedName);
            Assert.Equal(cpu.Price, result.Product.Price);
            Assert.Equal(cpu.Category, result.Product.Category);
        }

        /*
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