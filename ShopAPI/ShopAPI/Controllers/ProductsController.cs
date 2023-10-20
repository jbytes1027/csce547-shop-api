using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Data;
using ShopAPI.DTOs;
using ShopAPI.Models;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_context.Products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            // Convert to dto
            var dto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
                Manufacturer = product.Manufacturer
            };

            // Add the product's details depending on the Category
            switch (product.Category)
            {
                case Category.CPU:
                    var cpu = _context.CPUs.Where(c => c.ProductId == id).FirstOrDefault();
                    dto.Details = new Dictionary<string, string>
                    {
                        { "Cores", cpu.Cores.ToString() }
                    };
                    break;
                default:
                    return BadRequest("No category provided");
            }

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult PostProduct(ProductDTO dto)
        {
            // Create a new product
            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price,
                Description = dto.Description,
                Manufacturer = dto.Manufacturer
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            // Add the product's details depending on the Category
            switch (dto.Category)
            {
                case Category.CPU:
                    int v = int.Parse(dto.Details["Cores"]);
                    var cpu = new CPU
                    {
                        Cores = v,
                        ProductId = product.Id
                    };
                    _context.CPUs.Add(cpu);
                    _context.SaveChanges();
                    break;
                default:
                    return BadRequest("No category provided");
            }

            return GetProduct(product.Id);
        }
    }
}