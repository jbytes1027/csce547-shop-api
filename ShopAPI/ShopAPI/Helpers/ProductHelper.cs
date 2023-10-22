using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class ProductHelper
    {
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
            switch (category)
            {
                case Category.Cpu:
                    return new[] { "socket", "cores", "series", "integratedGraphics" };
                case Category.Case:
                    return new[] { "color", "formFactor", "sidePanel", "powerSupply" };
                default:
                    return null;
            }
        }
    }
}
