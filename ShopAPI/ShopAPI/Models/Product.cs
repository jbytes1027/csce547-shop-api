using Newtonsoft.Json;

namespace ShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
    }

    public class Case : Product
    {
        public string Color { get; set; }
        public string FormFactor { get; set; }
        public string SidePanel { get; set; }
        public bool PowerSupply { get; set; }
    }

    public class Cpu : Product
    {
        public string Socket { get; set; }
        public int Cores { get; set; }
        public string Series { get; set; }
        public bool IntegratedGraphics { get; set; }
    }
    public enum Category
    {
        Cpu,
        CpuCooler,
        Motherboard,
        Memory,
        Storage,
        VideoCard,
        PowerSupply,
        Case
    }
}
