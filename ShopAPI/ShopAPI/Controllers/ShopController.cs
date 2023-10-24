using Humanizer;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IProductService _productService;

        public ShopController(IProductService productService)
        {
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
    }
}