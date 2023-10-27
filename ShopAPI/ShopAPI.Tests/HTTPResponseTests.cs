
using Microsoft.AspNetCore.Mvc.Testing;


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
            // Act
            var response = await _client.GetAsync("api/GetCart/1");
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            // Arrange
            CreateClient();
            // Act
            var response = await _client.GetAsync("api/GetTotals/1");
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task HTTPResponseGetAllItemsTest()
        {
            // Arrange
            CreateClient();
            // Act
            var response = await _client.GetAsync("api/Item/GetAllItems");
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}