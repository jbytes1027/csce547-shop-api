namespace ShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public int Stock { get; set; } = 0;
    }

    public class Case : Product
    {
        public string Color { get; set; } = string.Empty;
        public string FormFactor { get; set; } = string.Empty;
        public string SidePanel { get; set; } = string.Empty;
        public bool PowerSupply { get; set; }
    }

    public class Cpu : Product
    {
        public string Socket { get; set; } = string.Empty;
        public int Cores { get; set; }
        public string Series { get; set; } = string.Empty;
        public bool IntegratedGraphics { get; set; }
    }

    public class CpuCooler : Product
    {
       public string Socket { get; set; } = string.Empty;
       public bool IsWaterCooled { get; set; }
       public string Size { get; set; } = string.Empty; // set as string for now because sizes are usually represented as LxWxH
    }

    public class Motherboard : Product
    {
        public string Socket { get; set; } = string.Empty;
        public string Chipset { get; set; } = string.Empty;
        public string MemoryType { get; set; } = string.Empty;
        public string FormFactor { get; set; } = string.Empty;
    }

    public class Memory : Product
    {
        public string MemoryType { get; set; } = string.Empty;
        public int Speed { get; set; }
        public int Size { get; set; }
    }

    public class Storage : Product
    {
        public string ConnectionType { get; set; } = string.Empty; // SATA, NVMe, USB, or IDE (IDE not really used anymore)
        public string DriveType { get; set; } = string.Empty; // Flash/SSD/HDD
        public int Speed { get; set; }
        public int Size { get; set; }
    }

    public class VideoCard : Product
    {
       public string Series { get; set; } = string.Empty;
       public int ClockSpeed { get; set; }
       public int VramSize { get; set; }
    }

    public class PowerSupply : Product
    {
        public int Wattage { get; set; }
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
