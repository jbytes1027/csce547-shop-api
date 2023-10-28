using Microsoft.EntityFrameworkCore;
using ShopAPI.Models;

namespace ShopAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });

            // Give product index on category
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Category);

            modelBuilder.Entity<Cpu>()
                .ToTable("Cpus")
                .HasBaseType<Product>();

            modelBuilder.Entity<Case>()
                .ToTable("Cases")
                .HasBaseType<Product>();

            modelBuilder.Entity<CpuCooler>()
                .ToTable("CpuCoolers")
                .HasBaseType<Product>();

            modelBuilder.Entity<Motherboard>()
                .ToTable("Motherboards")
                .HasBaseType<Product>();

            modelBuilder.Entity<Memory>()
                .ToTable("Memory")
                .HasBaseType<Product>();

            modelBuilder.Entity<Storage>()
                .ToTable("Storage")
                .HasBaseType<Product>();

            modelBuilder.Entity<VideoCard>()
                .ToTable("VideoCards")
                .HasBaseType<Product>();

            modelBuilder.Entity<PowerSupply>()
                .ToTable("PowerSupplies")
                .HasBaseType<Product>();
            
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cpu> CPUs { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CpuCooler> CpuCoolers { get; set; }
        public DbSet<VideoCard> VideoCards { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Motherboard> Motherboards { get; set; }
        public DbSet<PowerSupply> PowerSupplies { get; set; }    
    }
}