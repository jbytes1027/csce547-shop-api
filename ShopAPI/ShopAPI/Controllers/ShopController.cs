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
            var products = await _productService.GetProdcutsAsync(null, null);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products.ModelsToDTO());
        }

        // GET: api/Item/Filter
        [HttpGet("Item/Filter/{category}")]
        public async Task<IActionResult> FilterProducts(string category, [FromQuery] string? searchTerm = null)
        {
            if (!Enum.TryParse(category, ignoreCase: true, out Category productCategory))
            {
                return BadRequest("Invalid category");
            }

            var products = await _productService.GetProdcutsAsync(productCategory, searchTerm);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products.ModelsToDTO());
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

            return Ok(product.ModelToDTO());
        }

        // POST: api/item
        [HttpPost]
        [Route("Item")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid product data");
            }

            if (!Enum.TryParse(dto.Category, ignoreCase: true, out Category productCategory))
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
            if (dto == null)
            {
                return BadRequest("Invalid Card details");
            }

            // Checks if cart exists
            var cart = await _cartService.GetCartItemsAsync(dto.CartId);
            if (cart == null)
            {
                return BadRequest("Cart does not exist");
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

            return Ok("Payment processed for " + Calculate.DefaultBill(cart).GetTotalsDTO().TaxTotal);
        }

        [HttpPost]
        [Route("AddItemToCart/{cartId}")]
        public async Task<ActionResult<CartItemDTO>> AddItemToCart([FromRoute] int cartId, [FromBody] AddItemDTO item)
        {
            Product? product = await _productService.GetProductAsync(item.Id);

            if (product is null)
            {
                return NotFound();
            }

            await _cartService.AddItemAsync(cartId, item.Id, item.Quantity);

            ProductDTO productDTO = product.ModelToDTO();
            CartItemDTO cartItemDTO = new(productDTO, item.Quantity);

            return Ok(cartItemDTO);
        }

        [HttpGet]
        [Route("GetCart/{cartId}")]
        public async Task<ActionResult<CartDTO>> GetCart(int cartId)
        {
            List<CartItem> cartItems = await _cartService.GetCartItemsAsync(cartId);
            var bill = Calculate.DefaultBill(cartItems);

            CartDTO cartDTO = new()
            {
                Id = cartId,
                Items = cartItems.ToDTO(),
                Totals = bill.GetTotalsDTO(),
            };

            return Ok(cartDTO);
        }

        [HttpGet]
        [Route("GetTotals/{cartId}")]
        public async Task<ActionResult<TotalsDTO>> GetTotals(int cartId)
        {
            List<CartItem> items = await _cartService.GetCartItemsAsync(cartId);

            var bill = Calculate.DefaultBill(items);
            return Ok(bill.GetTotalsDTO());
        }

        [HttpGet]
        [Route("GetBill/{cartId}")]
        public async Task<ActionResult<Bill>> GetBill(int cartId)
        {
            List<CartItem> items = await _cartService.GetCartItemsAsync(cartId);

            var bill = Calculate.DefaultBill(items);
            return Ok(bill.ToJson());
        }
    }
}