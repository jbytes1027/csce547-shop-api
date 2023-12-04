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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // POST: api/processpayment
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] CardDTO dto)
        {
            // Checks if cart exists
            var cart = await _cartService.GetCartItemsAsync(dto.CartId);

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

            // Make sure we have enough stock
            foreach (var item in cart)
            {
                var product = item.Product;
                if (product.Stock < item.Quantity)
                {
                    return BadRequest("Not enough stock for " + product.Name);
                }
            }

            await _cartService.UpdateInventoryAfterPurchase(dto.CartId);

            await _cartService.ClearCart(dto.CartId);

            // Get the total with taxes
            decimal grandTotal = Calculate.DefaultBill(cart).GetTotalsDTO().TaxTotal;
            await _cartService.ClearCart(dto.CartId);
            return Ok("Payment processed for " + grandTotal);
        }

        // POST: api/AddItemToCart/{cartId}
        [HttpPost]
        [Route("AddItemToCart/{cartId}")]
        public async Task<ActionResult<CartItemDTO>> AddItemToCart([FromRoute] int cartId, [FromBody] AddItemDTO item)
        {
            if (item.Quantity < 0)
            {
                return BadRequest("Quantity must be positive");
            }

            var cartItem = await _cartService.AddItemAsync(cartId, item.Id, item.Quantity);

            return Ok(cartItem.ToDTO());
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
            return Ok(bill.ToDTO());
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

        [HttpPost]
        [Route("Cart/ClearCart/{cartId}")]
        public async Task<ActionResult> ClearCart(int cartId)
        {
            var cart = _cartService.GetCart(cartId).Result;

            if (cart is null)
            {
                return NotFound();
            }

            await _cartService.ClearCart(cartId);

            return NoContent();
        }

        [HttpDelete]
        [Route("Cart/RemoveCart/{cartId}")]
        public async Task<ActionResult<CartDTO>> DeleteCart(int cartId)
        {
            var cartSearch = await _cartService.GetCart(cartId);
            if (cartSearch == null)
            {
                return BadRequest("Cart does not exist");
            }
            else
            {
                await _cartService.RemoveCart(cartId);
                return Ok("Cart removed");
            }
        }
    }
}
