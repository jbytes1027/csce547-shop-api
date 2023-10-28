namespace ShopAPI.Models
{
    public record Total
    {
        public TotalType Type { get; set; }
        public decimal Value { get; set; }
    }

    public enum TotalType
    {
        BaseTotal,
        BundleTotal,
        TaxTotal,
    }
}