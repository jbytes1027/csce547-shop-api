
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using ShopAPI.Models;
using Newtonsoft.Json.Linq;

namespace ShopAPI.Tests
{
    public class HTTPResponseTests : IClassFixture<WebApplicationFactory<Program>>
    {
        // Arrange
        readonly HttpClient _client;
        public HTTPResponseTests(WebApplicationFactory<Program> app)
        {
            _client = app.CreateClient();
        }

        [Fact]
        public async Task HTTPResponseGetCartTest()
        {
            // Act
            var response = await _client.GetAsync("api/GetCart/1");
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            // Act
            var response = await _client.GetAsync("api/GetTotals/1");
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task HTTPResponseGetAllItemsTest()
        {
            // Act
            var response = await _client.GetAsync("api/Item/GetAllItems");
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}