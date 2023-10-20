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

    public enum Category
    {
        Case = 0,
        CPU = 1,
        CPUCooler = 2,
        Memory = 3,
        Motherboard = 4,
        PowerSupply = 5,
        Storage = 6,
        VideoCard = 7
    }
}
