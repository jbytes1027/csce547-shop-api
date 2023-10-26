using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShopAPI.DTOs;
using ShopAPI.Interfaces;
using ShopAPI.Mappers;
using ShopAPI.Models;
using ShopAPI.Services;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly IProductService _productService;

        public CartController(CartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        [HttpPost]
        [Route("{cartId}")]
        public async Task<ActionResult<CartItemDTO>> AddItemToCart([FromRoute] int cartId, [FromBody] AddItemDTO item)
        {
            Console.WriteLine(cartId);
            Console.WriteLine(item.ToJson());
            Console.WriteLine(item.Id);
            Console.WriteLine(item.Quantity);
            Product? product = await _productService.GetProductAsync(item.Id);

            if (product is null)
            {
                return NotFound();
            }

            await _cartService.addItemAsync(cartId, item.Id, item.Quantity);

            ProductDTO productDTO = product.ModelToDTO();
            CartItemDTO cartItemDTO = new()
            {
                CartId = cartId,
                Quantity = item.Quantity, // returned added quantity, not current quantity
                Id = item.Id,
                Name = productDTO.Name,
                Category = productDTO.Category,
                Price = productDTO.Price,
                Description = productDTO.Description,
                Manufacturer = productDTO.Manufacturer,
                Details = productDTO.Details,
            };

            return Ok(cartItemDTO);
        }
    }
}