
using Microsoft.AspNetCore.Mvc.Testing;
using ShopAPI.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ShopAPI.Tests
{
    public class HTTPResponseTests : IClassFixture<WebApplicationFactory<Program>>
    {
        HttpClient _client;
        private readonly ITestOutputHelper output;
        public HTTPResponseTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        internal void CreateClient()
        {
            WebApplicationFactory<Program> app = new WebApplicationFactory<Program>();
            _client = app.CreateClient();
        }
        
        // Helper function - Deletes all TEST Products
        private async Task DisposeAsync()
        {
            CreateClient();

            // Use Filter to get HttpResponse containing all Test Products
            var response = await _client.GetAsync("api/Item/Filter/PowerSupply?searchTerm=Test");
            // Parse jsonContent into Array
            try
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonContent);
                output.WriteLine("DELETING TEST PRODUCTS");
                foreach (JObject jsonObject in jsonArray)
                {
                    // Extract each product id and Delete the product
                    string id = (string)jsonObject["id"];
                    await _client.DeleteAsync("api/Item/" + id);
                    output.WriteLine("PRODUCT[" + id + "] DELETED");
                }
            }
            catch { };
        }

        [Fact]
        public async Task HTTPResponseGetAllItemsTest()
        {
            // Arrange
            CreateClient();
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            await _client.PostAsJsonAsync("api/Item", testDTO);
            var response = await _client.GetAsync("api/Item/GetAllItems");

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseFilterProductsTest()
        {
            // Arrange
            CreateClient();
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            await _client.PostAsJsonAsync("api/Item", testDTO);
            var response = await _client.GetAsync("api/Item/Filter/" + testDTO.Category + "?searchTerm=Test");

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseGetItemTest()
        {
            // Arrange
            CreateClient();
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse postResponse for product id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            string id = jsonDocument.RootElement.GetProperty("id").ToString();
            var response = await _client.GetAsync("api/Item/"+id);

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseDeleteItemTest()
        {
            // Arrange
            CreateClient();
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse postResponse for product id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            string id = jsonDocument.RootElement.GetProperty("id").ToString();
            // Delete Product, Expected response code: 200
            var response1 = await _client.DeleteAsync("api/Item/" + id);
            // Attempt to get product, Expected response code: 404
            var response2 = await _client.GetAsync("api/Item/" + id);

            // Assert
            response1.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
        }
        [Fact]
        public async Task HTTPResponseAddItemTest()
        {
            // Arrange
            CreateClient();
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            var response = await _client.PostAsJsonAsync("api/Item", testDTO);

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseProcessPaymentTest()
        {
            // Arrange
            CreateClient();
            int cartID = 777;
            // Product DTO to Add Item
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;
            // Card DTO to process Payment
            CardDTO cardDTO = new CardDTO();
            cardDTO.CardHolderName = "TEST";
            cardDTO.CardNumber = 1234123412341234;
            cardDTO.Exp = "0129";
            cardDTO.CartId = cartID;
            cardDTO.Cvv = 777;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse product ID for later use
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItemt DTO to add Item to Cart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            // Add Item To Cart
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartID.ToString(), addItemDTO);
            var paymentResponse = await _client.PostAsJsonAsync("api/ProcessPayment", cardDTO);

            // Assert
            paymentResponse.EnsureSuccessStatusCode();
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseAddItemToCartTest()
        {
            // Arrange
            CreateClient();
            int cartID = 777;
            // Product DTO to addItem
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse product ID for later use
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItem DTO for AddToCart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            var response = await _client.PostAsJsonAsync("api/AddItemToCart/" + cartID.ToString(), addItemDTO);

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseGetCartTest()
        {
            // Arrange
            CreateClient();
            int cartID = 777;
            // Product DTO to addItem
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse product ID for later use
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItem DTO for AddToCart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartID.ToString(), addItemDTO);
            var response = await _client.GetAsync("api/GetCart/" + cartID.ToString());
            String responseBody = await response.Content.ReadAsStringAsync();
            // Pull ID from GetCart for comparison
            JObject jsonObject = JObject.Parse(responseBody);
            int idGetCart = (int)jsonObject["items"][0]["id"];

            // Assert
            Assert.Equal(productId, idGetCart);
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            // Arrange
            CreateClient();
            int expectedBaseTotal = 3000;
            int cartID = 777;
            // Product DTO to addItem
            ProductDTO testDTO = new ProductDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;

            // Act
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse product ID for later use
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItem DTO for AddToCart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartID.ToString(), addItemDTO);
            var response = await _client.GetAsync("api/GetTotals/" + cartID.ToString());
            // Parse Cart Total for comparison
            string jsonContentTotal = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDocumentTotal = JsonDocument.Parse(jsonContentTotal);
            int baseTotal = Convert.ToInt32(jsonDocumentTotal.RootElement.GetProperty("BaseTotal").ToString());

            // Assert
            Assert.Equal(expectedBaseTotal, baseTotal);
            await DisposeAsync();
        }
    }
}