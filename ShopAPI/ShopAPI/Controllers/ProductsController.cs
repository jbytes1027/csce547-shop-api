using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Helpers;
using ShopAPI.Interfaces;
using ShopAPI.Mappers;
using ShopAPI.Models;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products/
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? category = null, [FromQuery] string? searchString = null)
        {
            Category? productCategory = null;

            // Try to parse the category
            // If the category is invalid, return a bad request
            if (category != null)
            {
                productCategory = ProductHelper.TryGetCategory(category);

                if (productCategory == null)
                {
                    return BadRequest("Invalid category");
                }
            }

            var products = await _productService.GetProdcutsAsync(productCategory, searchString);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products.ModelsToDTO());
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ModelToDTO());
        }

        // POST: api/products
        [HttpPost]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.RemoveProductAsync(id);

            return NoContent();
        }
    }
}