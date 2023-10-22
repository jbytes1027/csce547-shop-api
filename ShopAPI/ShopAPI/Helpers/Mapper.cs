using ShopAPI.DTOs;
using ShopAPI.Models;

namespace ShopAPI.Mappers
{
    public static class Mapper
    {
        /// <summary>
        /// Converts a ProductDTO to the base Product type based on the specified category.
        /// </summary>
        /// <param name="dto">The ProductDTO to convert.</param>
        /// <param name="category">The category of the product.</param>
        /// <returns>The base Product object with properties populated from the DTO.</returns>
        /// <exception cref="NotImplementedException">Thrown when the provided category is not supported.</exception>
        public static Product ToBaseProduct(this ProductDTO dto, Category category)
        {
            Product product = category switch
            {
                Category.Cpu => new Cpu(),
                Category.Case => new Case(),
                _ => throw new NotImplementedException()
            };

            product.Name = dto.Name;
            product.Category = category;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.Manufacturer = dto.Manufacturer;

            return product;
        }

        /// <summary>
        /// Converts a Product object to a ProductDTO, including additional details based on the product type.
        /// </summary>
        /// <param name="product">The Product object to convert.</param>
        /// <returns>A ProductDTO representing the converted product.</returns>
        /// <exception cref="NotImplementedException">Thrown when the product type is not supported.</exception>
        public static ProductDTO ModelToDTO(this Product product)
        {
            ProductDTO dto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category.ToString(),
                Price = product.Price,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                Details = new Dictionary<string, string>()
            };

            switch (product)
            {
                case Cpu cpu:
                    dto.Details.Add("stocket", cpu.Socket);
                    dto.Details.Add("cores", cpu.Cores.ToString());
                    dto.Details.Add("series", cpu.Series.ToString());
                    dto.Details.Add("integratedGraphics", cpu.IntegratedGraphics.ToString());
                    break;
                case Case case_:
                    dto.Details.Add("color", case_.Color);
                    dto.Details.Add("formFactor", case_.FormFactor);
                    dto.Details.Add("sidePanel", case_.SidePanel);
                    dto.Details.Add("powerSupply", case_.PowerSupply.ToString());
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dto;
        }

        /// <summary>
        /// Converts a collection of Product objects to a collection of ProductDTO objects.
        /// </summary>
        /// <param name="products">The collection of Product objects to convert.</param>
        /// <returns>A collection of ProductDTO representing the converted products.</returns>
        public static IEnumerable<ProductDTO> ModelsToDTO(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ModelToDTO());
        }
    }
}
