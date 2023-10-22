using ShopAPI.Models;
using System.Reflection;

namespace ShopAPI.Helpers
{
    public static class ProductHelper
    {
        // L
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

        public static Category? TryGetCategory(string? category)
        {
            if (Enum.TryParse(category, ignoreCase: true, out Category productCategory))
            {
                return productCategory;
            }

            return null;
        }

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

                default:
                    return null;
            }

            return requiredProperties.Except(ignoreProperties, StringComparer.OrdinalIgnoreCase);
        }
    }
}
