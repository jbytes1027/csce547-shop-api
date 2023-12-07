using Microsoft.AspNetCore.Mvc.Testing;
using ShopAPI.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using ShopAPI.Models;
using Microsoft.CodeAnalysis;

namespace ShopAPI.Tests
{
    public class HTTPResponseTests : IClassFixture<WebApplicationFactory<Program>>
    {
        int numTestItems = 3;   // # Items in Cart
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

        // Helper function - Arrange Inventory info
        private async Task<(ProductDTO,HttpResponseMessage)> ArrangeTestProductDTO()
        {
            // Create Product DTO
            ProductDTO prodDTO = new ProductDTO();
            Dictionary<string, string> prodDetails = new Dictionary<string, string> { };
            prodDetails.Add("Wattage", "950");
            prodDTO.Category = "PowerSupply";
            prodDTO.Manufacturer = "TEST";
            prodDTO.Name = "TEST";
            prodDTO.Details = prodDetails;
            prodDTO.Stock = numTestItems;
            prodDTO.Price = 1000;
            // Add Item To Inventory
            var response = await _client.PostAsJsonAsync("api/Inventory/AddNewItem", prodDTO);
            return (prodDTO,response);
        }

        // Helper function - Arrange Credit-Card info
        private CardDTO ArrangeTestCardDTO()
        {
            CardDTO cardDTO = new CardDTO();
            cardDTO.CardHolderName = "TEST";
            cardDTO.CardNumber = 1234123412341234;
            cardDTO.Exp = "0129";
            cardDTO.Cvv = 777;
            return cardDTO;

        }

        // Helper function - Arrange CartDTO
        private async Task<(CartDTO, HttpResponseMessage)> ArrangeTestCartDTO()
        {
            CartDTO cartDTO = new CartDTO();
            cartDTO.Name = "TEST";
            var response = await _client.PostAsJsonAsync("api/Cart/CreateNewCart", cartDTO);
            return (cartDTO, response);
        }

        /* Helper Function 
         *  - Creates Products and a Cart
         *  - Adds Items to Cart if "itemsInCart" = True
         *  returns: (Cart ID,Product ID)
         * */
        private async Task<(int,int)> ArrangeTestCart(bool itemsInCart)
        {
            var (prodDTO, itemPostResponse) = await ArrangeTestProductDTO();
            var (cartDTO, cartPostResponse) = await ArrangeTestCartDTO();
            // Parse Json Response for Cart Id
            string jCartContent = await cartPostResponse.Content.ReadAsStringAsync();
            JsonDocument jCartResponse = JsonDocument.Parse(jCartContent);
            int cartId = Convert.ToInt32(jCartResponse.RootElement.GetProperty("id").ToString());
            // Parse Json response for product Id
            string jsonContent = await itemPostResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            int productId = Convert.ToInt32(jsonDocument.RootElement.GetProperty("id").ToString());
            if (itemsInCart)
            {
                // Add product to the Cart
                AddItemDTO addItemDTO = new AddItemDTO();
                addItemDTO.Id = productId;
                addItemDTO.Quantity = numTestItems;
                await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);
            }

            return (cartId,productId);
        }
        
        // Helper function - Deletes all items added to inventory
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
            /* Arrange */ 
            CreateClient();
            var (_,_) = await ArrangeTestProductDTO();

            /* Act */ 
            var response = await _client.GetAsync("api/Item/GetAllItems");

            /* Assert */
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseFilterProductsTest()
        {
            /* Arrange */
            CreateClient();
            var (prodDTO, _) = await ArrangeTestProductDTO();

            /* Act */
            var response = await _client.GetAsync("api/Item/Filter/" + prodDTO.Category + "?searchTerm=Test");

            /* Assert */
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseGetItemTest()
        {
            /* Arrange */
            CreateClient();
            var (_, postResponse) = await ArrangeTestProductDTO();
            // Parse post Http Response for product id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            string id = jsonDocument.RootElement.GetProperty("id").ToString();

            /* Act */
            var response = await _client.GetAsync("api/Item/"+id);

            /* Assert */
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseDeleteItemTest()
        {
            /* Arrange */
            CreateClient();
            var (_, postResponse) = await ArrangeTestProductDTO();
            // Parse postResponse for product id
            string jsonContent = await postResponse.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
            string id = jsonDocument.RootElement.GetProperty("id").ToString();

            /* Act */
            // Delete Product, Expected response code: 200
            var response1 = await _client.DeleteAsync("api/Item/" + id);
            // Attempt to get product, Expected response code: 404
            var response2 = await _client.GetAsync("api/Item/" + id);

            /* Assert */
            response1.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
        }

        [Fact]
        public async Task HTTPResponseAddItemTest()
        {
            /* Arrange */
            CreateClient();
            // Create new Product DTO
            ProductDTO prodDTO = new ProductDTO();
            Dictionary<string, string> prodDetails = new Dictionary<string, string> { };
            prodDetails.Add("Wattage", "950");
            prodDTO.Category = "PowerSupply";
            prodDTO.Manufacturer = "TEST";
            prodDTO.Name = "TEST";
            prodDTO.Details = prodDetails;
            prodDTO.Stock = numTestItems;
            /* Act */
            var response = await _client.PostAsJsonAsync("api/Inventory/AddNewItem", prodDTO);

            /* Assert */
            response.EnsureSuccessStatusCode();
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseProcessPaymentTest()
        {
            /* Arrange */
            CreateClient();
            var (cartID,_) = await ArrangeTestCart(true);
            CardDTO cardDTO = ArrangeTestCardDTO();
            cardDTO.CartId = cartID;

            /* Act */
            var paymentResponse = await _client.PostAsJsonAsync("api/ProcessPayment", cardDTO);

            /* Assert */
            paymentResponse.EnsureSuccessStatusCode();

            // Clear Cart & Inventory
            await _client.DeleteAsync("api/cart/DeleteCart" + cartID.ToString());
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseAddItemToCartTest()
        {
            /* Arrange */
            CreateClient();
            var (cartId,prodId) = await ArrangeTestCart(false);
            // Create new AddItem DTO
            AddItemDTO addItemDTO = new AddItemDTO();
            addItemDTO.Id = prodId;
            addItemDTO.Quantity = numTestItems;

            /* Act */
            var response = await _client.PostAsJsonAsync("api/AddItemToCart/" + cartId.ToString(), addItemDTO);

            /* Assert */
            response.EnsureSuccessStatusCode();

            // Clear Cart & Inventory
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
            await DisposeAsync();
        }
        [Fact]
        public async Task HTTPResponseGetCartTest()
        {
            /* Arrange */
            CreateClient();
            var (cartId,prodId) = await ArrangeTestCart(true);

            /* Act */         
            var response = await _client.GetAsync("api/GetCart/" + cartId.ToString());
            // Parse GetCart Response for product Id 
            String responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(responseBody);
            int idGetCart = (int)jsonObject["items"][0]["id"];

            /* Assert */
            Assert.Equal(prodId, idGetCart);

            // Clear Cart & Inventory
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
            await DisposeAsync();
        }

        [Fact]
        public async Task HTTPResponseGetTotalsTest()
        {
            /* Arrange */
            CreateClient();
            int expectedBaseTotal = 3000;
            var (cartId, _) = await ArrangeTestCart(true);

            /* Act */
            var response = await _client.GetAsync("api/GetTotals/" + cartId.ToString());
            // Parse HTTP Response for Cart Total
            string jsonContentTotal = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDocumentTotal = JsonDocument.Parse(jsonContentTotal);
            int baseTotal = Convert.ToInt32(jsonDocumentTotal.RootElement.GetProperty("baseTotal").ToString());

            /* Assert */
            Assert.Equal(expectedBaseTotal, baseTotal);

            // Clear Cart & Inventory
            await _client.DeleteAsync("api/cart/DeleteCart" + cartId.ToString());
            await DisposeAsync();
        }
    }
}