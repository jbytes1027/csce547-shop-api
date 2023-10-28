
using Microsoft.AspNetCore.Mvc.Testing;
using ShopAPI.Models;

namespace ShopAPI.Tests
{
    public class HTTPResponseTests : IClassFixture<WebApplicationFactory<Program>>
    {
        HttpClient _client;

        internal void CreateClient()
        {
            WebApplicationFactory<Program> app = new WebApplicationFactory<Program>();
            _client = app.CreateClient();
        }

        [Fact]
        public async Task HTTPResponseGetCartTest()
        {
            // Arrange
            CreateClient();
            CartItem cart = new CartItem();
            cart.CartId = 7;
            cart.Product = new Product();
            cart.Product.Id = 1;
            // Act
            var response = await _client.GetAsync("api/GetCart/"+cart.CartId);
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            // Arrange
            CreateClient();
            CartItem cart = new CartItem();
            cart.CartId = 7;
            cart.Product = new Product();
            cart.Product.Id = 1;
            // Act
            var response = await _client.GetAsync("api/GetTotals/1");
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}