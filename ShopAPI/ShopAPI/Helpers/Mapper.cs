using ShopAPI.DTOs;
using ShopAPI.Models;

namespace ShopAPI.Mappers
{
    public static class Mapper
    {
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

        public static IEnumerable<ProductDTO> ModelsToDTO(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ModelToDTO());
        }
    }
}
