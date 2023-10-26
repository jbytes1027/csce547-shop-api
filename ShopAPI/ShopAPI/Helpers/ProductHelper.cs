using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class ProductHelper
    {
        /// <summary>
        /// Validates product details based on the specified category and a dictionary of details.
        /// </summary>
        /// <param name="category">The category of the product.</param>
        /// <param name="details">A dictionary containing details to be validated.</param>
        /// <returns>
        /// If the category is invalid, returns "No category found."
        /// If there are missing properties in the details, returns an error message listing the required properties.
        /// If the details are valid, returns null.
        public static string? ValidateProductDetails(Category category, Dictionary<string, string> details)
        {
            var requiredProperties = GetRequiredPropertiesForCategory(category);

            if (requiredProperties == null)
            {
                return "No category found";
            }

            var missingProperties = requiredProperties.Except(details.Keys, StringComparer.OrdinalIgnoreCase).ToList();

            if (missingProperties.Any())
            {
                return $"Invalid {category} details. Correct details should contain: {string.Join(", ", missingProperties)}";
            }

            return null;

        }

        /// <summary>
        /// Tries to parse a string into a Category enum. Returns the parsed Category or null if unsuccessful.
        /// </summary>
        /// <param name="category">The string representation of the category.</param>
        /// <returns>The parsed Category or null if parsing fails.</returns>
        public static Category? TryGetCategory(string? category)
        {
            if (Enum.TryParse(category, ignoreCase: true, out Category productCategory))
            {
                return productCategory;
            }

            return null;
        }

        /// <summary>
        /// Gets the required properties for a specified category.
        /// </summary>
        /// <param name="category">The category for which to retrieve required properties.</param>
        /// <returns>
        /// A collection of required property names for the specified category or null if the category is not recognized.
        /// </returns>
        private static IEnumerable<string>? GetRequiredPropertiesForCategory(Category category)
        {
            IEnumerable<string> requiredProperties;
            var ignoreProperties = typeof(Product).GetProperties().Select(p => p.Name);
            

            switch (category)
            {
                case Category.Cpu:
                    var cpuProperties = typeof(Cpu).GetProperties();
                    requiredProperties = cpuProperties.Select(p => p.Name);
                    break;
                
                case Category.Case:
                    var caseProperties = typeof(Case).GetProperties();
                    requiredProperties = caseProperties.Select(p => p.Name);
                    break;
                
                case Category.CpuCooler:
                    var cpuCoolerProperties = typeof(CpuCooler).GetProperties();
                    requiredProperties = cpuCoolerProperties.Select(p => p.Name);
                    break;
                
                case Category.Motherboard:
                    var motherboardProperties = typeof(Motherboard).GetProperties();
                    requiredProperties = motherboardProperties.Select(p => p.Name);
                    break;
                
                case Category.Memory:
                    var memoryProperties = typeof(Memory).GetProperties();
                    requiredProperties = memoryProperties.Select(p => p.Name);
                    break;
                                
                case Category.Storage:
                    var storageProperties = typeof(Storage).GetProperties();
                    requiredProperties = storageProperties.Select(p => p.Name);
                    break;
                                
                case Category.VideoCard:
                    var videoCardProperties = typeof(VideoCard).GetProperties();
                    requiredProperties = videoCardProperties.Select(p => p.Name);
                    break;
                 
                case Category.PowerSupply:
                    var powerSupplyProperties = typeof(PowerSupply).GetProperties();
                    requiredProperties =powerSupplyProperties.Select(p => p.Name);
                    break;
                 
                default:
                    return null;
            }

            return requiredProperties.Except(ignoreProperties, StringComparer.OrdinalIgnoreCase);
        }
    }
}
