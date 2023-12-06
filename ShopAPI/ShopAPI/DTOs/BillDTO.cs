using ShopAPI.Helpers;

namespace ShopAPI.DTOs
{
    public class BillDTO
    {
        public List<Surcharge> BaseCharges { get; set; } = new();
        public List<Surcharge> BundleSurcharges { get; set; } = new();
        public List<Surcharge> TaxSurcharges { get; set; } = new();
    }
}