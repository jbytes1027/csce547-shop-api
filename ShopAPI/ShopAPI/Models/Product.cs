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
        public string NormalizedName { get; set; }
        public string NormalizedDescription { get; set; }
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
    
    public class CpuCooler : Product
    {
       public string Socket { get; set; } 
       public bool IsWaterCooled { get; set; }
       public string Size { get; set; } // set as string for now because sizes are usually represented as LxWxH
    }

    public class Motherboard : Product
    {
        public string Socket { get; set; }
        public string Chipset { get; set; }
        public string MemoryType { get; set; }
        public string FormFactor { get; set; }
    }

    public class Memory : Product
    {
        public string MemoryType { get; set; }
        public int Speed { get; set; }
        public int Size { get; set; }
    }

    public class Storage : Product
    {
        public string ConnectionType { get; set; } // SATA, NVMe, USB, or IDE (IDE not really used anymore)
        public string DriveType { get; set; } // Flash/SSD/HDD
        public int Speed { get; set; } 
        public int Size { get; set; }
    }

    public class VideoCard : Product
    {
       public string Series { get; set; }
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
