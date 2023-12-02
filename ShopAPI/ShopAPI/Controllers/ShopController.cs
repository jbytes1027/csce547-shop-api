using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShopAPI.DTOs;
using ShopAPI.Helpers;
using ShopAPI.Interfaces;
using ShopAPI.Mappers;
using ShopAPI.Models;

namespace ShopAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public ShopController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // GET: api/Item/GetAllItems
        [HttpGet("Item/GetAllItems")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.SearchProductsAsync();

            return Ok(products.ToDTO());
        }

        // GET: api/Item/Filter
        [HttpGet("Item/Filter/{category}")]
        public async Task<IActionResult> FilterProducts(string category, [FromQuery] string keyword = "")
        {
            // Try to get category enum from category string
            bool parseSuccess = Enum.TryParse(category, ignoreCase: true, out Category productCategory);

            if (!parseSuccess)
            {
                return NotFound("Invalid category");
            }

            var products = await _productService.SearchProductsAsync(productCategory, keyword);

            return Ok(products.ToDTO());
        }

        // GET: api/Item/{id}
        [HttpGet("Item/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ToDTO());
        }

        // POST: api/Item
        [HttpPost]
        [Route("Item")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            // Try to get category enum from category string
            bool parseSuccess = Enum.TryParse(dto.Category, ignoreCase: true, out Category productCategory);

            if (!parseSuccess)
            {
                return BadRequest("Invalid category");
            }

            // Create a new base product entity
            var baseProduct = dto.ToBaseProduct(productCategory);

            // Make sure the product details are valid
            var validationError = ProductHelper.ValidateProductDetails(productCategory, dto.Details);

            // If the product details are invalid, return a bad request
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            // Add the product to the database
            var product = await _productService.CreateProductAsync(baseProduct, dto.Details);

            if (product == null)
            {
                return BadRequest("Invalid product data");
            }

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // DELETE: api/products/{id}
        [HttpDelete("Item/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Find the product
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.RemoveProductAsync(id);

            return NoContent();
        }

        // POST: api/processpayment
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] CardDTO dto)
        {
            // Checks if cart exists
            var cart = await _cartService.GetCartItemsAsync(dto.CartId);
            if (cart == null)
            {
                return BadRequest("Cart does not exist");
            }

            // Cart is empty, nothing to process
            if (!cart.Any())
            {
                return BadRequest("Cart is empty");
            }

            // Field checking
            if (dto.CardNumber.ToString().Length != 16)
            {
                return BadRequest("Card length incorrect");
            }

            if (dto.Cvv.ToString().Length != 3)
            {
                return BadRequest("CVV length incorrect");
            }

            if (string.IsNullOrEmpty(dto.Cvv.ToString()))
            {
                return BadRequest("CVV field empty");
            }

            if (string.IsNullOrEmpty(dto.CardHolderName))
            {
                return BadRequest("Holder name field empty");
            }

            if (string.IsNullOrEmpty(dto.CartId.ToString()))
            {
                return BadRequest("Card ID field empty");
            }

            if (string.IsNullOrEmpty(dto.CardNumber.ToString()))
            {
                return BadRequest("Card Number field empty");
            }

            if (string.IsNullOrEmpty(dto.Exp))
            {
                return BadRequest("Expiration date field empty");
            }


            /*
            // TODO(epadams) Checking date more thoroughly, maybe split
            try
            {
                var date = DateTime.Parse(dto.Exp).ToString("MM/y");
                Console.WriteLine(date);
            }
            catch (FormatException)
            {
                return BadRequest("Unable to parse date");
            }
            */

            // Get the total with taxes
            var bill = Calculate.DefaultBill(cart).GetTotalsDTO().TaxTotal;

            await _cartService.ClearCart(dto.CartId);

            return Ok("Payment processed for " + bill);
        }

        // POST: api/AddItemToCart/{cartId}
        [HttpPost]
        [Route("AddItemToCart/{cartId}")]
        public async Task<ActionResult<CartItemDTO>> AddItemToCart([FromRoute] int cartId, [FromBody] AddItemDTO item)
        {
            Product? product = await _productService.GetProductAsync(item.Id);

            if (product is null)
            {
                return NotFound();
            }

            if (item.Quantity < 0)
            {
                return BadRequest("Quantity must be positive");
            }

            var newItem = await _cartService.AddItemAsync(cartId, item.Id, item.Quantity);
            if (newItem is null)
            {
                return NotFound("Cart not found");
            }

            var cartItemDTO = new CartItemDTO(product.ToDTO(), item.Quantity);

            return Ok(cartItemDTO);
        }

        // GET: api/GetCart/{cartId}
        [HttpGet]
        [Route("GetCart/{cartId}")]
        public async Task<ActionResult<CartDTO>> GetCart(int cartId)
        {
            Cart? cart = await _cartService.GetCart(cartId);
            if (cart is null)
            {
                return NotFound();
            }

            var bill = Calculate.DefaultBill(cart.Items);

            CartDTO cartDTO = new()
            {
                Id = cartId,
                Name = cart.Name,
                Items = cart.Items.ToDTO(),
                Totals = bill.GetTotalsDTO(),
            };

            return Ok(cartDTO);
        }

        // GET: api/GetTotals/{cartId}
        [HttpGet]
        [Route("GetTotals/{cartId}")]
        public async Task<ActionResult<TotalsDTO>> GetTotals(int cartId)
        {
            List<CartItem> items = await _cartService.GetCartItemsAsync(cartId);

            var bill = Calculate.DefaultBill(items);
            return Ok(bill.GetTotalsDTO());
        }

        // GET: api/GetBill/{cartId}
        [HttpGet]
        [Route("GetBill/{cartId}")]
        public async Task<ActionResult<Bill>> GetBill(int cartId)
        {
            List<CartItem> items = await _cartService.GetCartItemsAsync(cartId);

            var bill = Calculate.DefaultBill(items);
            Console.WriteLine(bill);
            return Ok(bill.ToJson());
        }

        [HttpPost]
        [Route("Cart/CreateNewCart")]
        public async Task<IActionResult> CreateNewCart(Cart cart)
        {
            var newCart = await _cartService.CreateCartAsync(cart.Name);
            return Ok(newCart);
        }

        [HttpDelete]
        [Route("Cart/RemoveItem/{cartId}")]
        public async Task<ActionResult<CartDTO>> RemoveItemFromCart([FromRoute] int cartId, CartRemoveItemRequest request)
        {
            // If request quantity is not specified, remove all the items from the cart
            request.Quantity ??= int.MaxValue;

            await _cartService.RemoveItemAsync(cartId, request.ItemId, (int)request.Quantity);

            return await GetCart(cartId); // Purposely deviates from http spec to matches client spec
        }

            return await GetCart(cartId);
        }

        [HttpDelete]
        [Route("Cart/ClearCart/{cartId}")]
        public ActionResult ClearCart(int cartId)
        {
            if (_cartService.GetCart(cartId) == null)
            {
                return BadRequest("Cart does not exist");
            }

            var cart = _cartService.GetCart(cartId).Result;

            if (cart is null)
            {
                return NotFound();
            }

            if (!cart.Items.Any())
            {
                return BadRequest("Cart is already empty");
            }

            _cartService.ClearCart(cartId);
            return Ok("Cart cleared");
        }
    }
}