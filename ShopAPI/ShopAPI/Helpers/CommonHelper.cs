namespace ShopAPI.Helpers
{
    public static class CommonHelper
    {
        /// <summary>
        /// Converts a string value to the specified target type.
        /// </summary>
        /// <param name="stringValue">The string value to convert.</param>
        /// <param name="targetType">The target type to convert the string to.</param>
        /// <returns>The converted value of the specified target type.</returns>
        public static object ConvertToTypedValue(string stringValue, Type targetType)
        {
            try
            {
                // Handle different target types
                if (targetType == typeof(int))
                {
                    return int.Parse(stringValue);
                }
                else if (targetType == typeof(string))
                {
                    return stringValue;
                }
                else if (targetType.IsEnum)
                {
                    return Enum.Parse(targetType, stringValue, ignoreCase: true);
                }
                else if (targetType == typeof(decimal))
                {
                    return decimal.Parse(stringValue);
                }
                else if (targetType == typeof(bool))
                {
                    return bool.Parse(stringValue);
                }

                return stringValue;
            }
            catch (Exception)
            {
                return new object();
            }
        }
    }
}
