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
            // Product DTO to Add Item
            ProductDTO testDTO = new ProductDTO();
            CartDTO cartDTO = new CartDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;
            // Create Cart 
            cartDTO.Name = "TEST";
            var cartResponse = await _client.PostAsJsonAsync("api/Cart/CreateNewCart", cartDTO);
            // Parse Response for Cart Id
            string jCartContent = await cartResponse.Content.ReadAsStringAsync();
            JsonDocument jCartResponse = JsonDocument.Parse(jCartContent);
            int cartId = Convert.ToInt32(jCartResponse.RootElement.GetProperty("id").ToString());
            // Card DTO to process Payment
            CardDTO cardDTO = new CardDTO();
            cardDTO.CardHolderName = "TEST";
            cardDTO.CardNumber = 1234123412341234;
            cardDTO.Exp = "0129";
            cardDTO.CartId = cartId;
            cardDTO.Cvv = 777;
            // Parse Json Response for product Id
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItem To Cart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);

            // Act
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
            // Product DTO to add to a Cart
            ProductDTO testDTO = new ProductDTO();
            CartDTO cartDTO = new CartDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Details = testDetails;
            // Create Cart 
            cartDTO.Name = "TEST";
            var cartResponse = await _client.PostAsJsonAsync("api/Cart/CreateNewCart",cartDTO);
            // Parse Response for Cart Id
            string jCartContent = await cartResponse.Content.ReadAsStringAsync();
            JsonDocument jCartResponse = JsonDocument.Parse(jCartContent);
            int cartId = Convert.ToInt32(jCartResponse.RootElement.GetProperty("id").ToString());
            // Create Item
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse response for product Id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // AddItem DTO for AddToCart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;

            // Act (Add Item to Cart)
            var response = await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);

            // Assert
            response.EnsureSuccessStatusCode();
            await DisposeAsync();

            // Delete Cart
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
        }
        [Fact]
        public async Task HTTPResponseGetCartTest()
        {
            // Arrange
            CreateClient();
            // Product DTO to addItem
            ProductDTO testDTO = new ProductDTO();
            CartDTO cartDTO = new CartDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;
            // Create Cart 
            cartDTO.Name = "TEST";
            var cartResponse = await _client.PostAsJsonAsync("api/Cart/CreateNewCart", cartDTO);
            // Parse Json Response for Cart Id
            string jCartContent = await cartResponse.Content.ReadAsStringAsync();
            JsonDocument jCartResponse = JsonDocument.Parse(jCartContent);
            int cartId = Convert.ToInt32(jCartResponse.RootElement.GetProperty("id").ToString());
            // Parse Json response for product Id
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // Add product to the Cart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);

            // Act         
            var response = await _client.GetAsync("api/GetCart/" + cartId.ToString());
            // Parse GetCart Response for product Id 
            String responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(responseBody);
            int idGetCart = (int)jsonObject["items"][0]["id"];

            // Assert
            Assert.Equal(productId, idGetCart);
            await DisposeAsync();

            // Delete Cart
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            // Arrange
            CreateClient();
            int expectedBaseTotal = 3000;
            // Product DTO to addItem
            ProductDTO testDTO = new ProductDTO();
            CartDTO cartDTO = new CartDTO();
            Dictionary<string, string> testDetails = new Dictionary<string, string> { };
            testDetails.Add("Wattage", "950");
            testDTO.Category = "PowerSupply";
            testDTO.Manufacturer = "TEST";
            testDTO.Name = "TEST";
            testDTO.Price = 1000;
            testDTO.Details = testDetails;
            // Create Cart 
            cartDTO.Name = "TEST";
            var cartResponse = await _client.PostAsJsonAsync("api/Cart/CreateNewCart", cartDTO);
            // Parse Json Response for Cart Id
            string jCartContent = await cartResponse.Content.ReadAsStringAsync();
            JsonDocument jCartResponse = JsonDocument.Parse(jCartContent);
            int cartId = Convert.ToInt32(jCartResponse.RootElement.GetProperty("id").ToString());
            var postResponse = await _client.PostAsJsonAsync("api/Item", testDTO);
            // Parse Json Response for Product Id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            // Add 3 of TEST product to cart
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = productId;
            addItemDTO.Quantity = 3;
            await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);

            // Act
            var response = await _client.GetAsync("api/GetTotals/" + cartId.ToString());
            // Parse Json Response for Cart Total
            string jsonContentTotal = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDocumentTotal = JsonDocument.Parse(jsonContentTotal);
            int baseTotal = Convert.ToInt32(jsonDocumentTotal.RootElement.GetProperty("baseTotal").ToString());

            // Assert
            Assert.Equal(expectedBaseTotal, baseTotal);
            await DisposeAsync();

            // Delete Cart
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
        }
    }
}