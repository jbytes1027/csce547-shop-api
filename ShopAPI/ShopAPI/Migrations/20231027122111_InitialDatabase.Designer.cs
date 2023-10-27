﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShopAPI.Data;

#nullable disable

namespace ShopAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231027122111_InitialDatabase")]
    partial class InitialDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShopAPI.Models.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("ShopAPI.Models.CartItem", b =>
                {
                    b.Property<int>("CartId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("CartId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("ShopAPI.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("Category");

                    b.ToTable("Products");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("ShopAPI.Models.Case", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FormFactor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PowerSupply")
                        .HasColumnType("boolean");

                    b.Property<string>("SidePanel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Cases", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.Cpu", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<int>("Cores")
                        .HasColumnType("integer");

                    b.Property<bool>("IntegratedGraphics")
                        .HasColumnType("boolean");

                    b.Property<string>("Series")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Socket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Cpus", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.CpuCooler", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<bool>("IsWaterCooled")
                        .HasColumnType("boolean");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Socket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("CpuCoolers", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.Memory", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<string>("MemoryType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.Property<int>("Speed")
                        .HasColumnType("integer");

                    b.ToTable("Memory", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.Motherboard", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<string>("Chipset")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FormFactor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MemoryType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Socket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Motherboards", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.PowerSupply", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<int>("Wattage")
                        .HasColumnType("integer");

                    b.ToTable("PowerSupplies", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.Storage", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<string>("ConnectionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DriveType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.Property<int>("Speed")
                        .HasColumnType("integer");

                    b.ToTable("Storage", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.VideoCard", b =>
                {
                    b.HasBaseType("ShopAPI.Models.Product");

                    b.Property<int>("ClockSpeed")
                        .HasColumnType("integer");

                    b.Property<string>("Series")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VramSize")
                        .HasColumnType("integer");

                    b.ToTable("VideoCards", (string)null);
                });

            modelBuilder.Entity("ShopAPI.Models.CartItem", b =>
                {
                    b.HasOne("ShopAPI.Models.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShopAPI.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShopAPI.Models.Case", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.Case", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.Cpu", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.Cpu", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.CpuCooler", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.CpuCooler", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.Memory", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.Memory", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.Motherboard", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.Motherboard", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.PowerSupply", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.PowerSupply", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.Storage", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.Storage", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.VideoCard", b =>
                {
                    b.HasOne("ShopAPI.Models.Product", null)
                        .WithOne()
                        .HasForeignKey("ShopAPI.Models.VideoCard", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAPI.Models.Cart", b =>
                {
                    b.Navigation("CartItems");
                });
#pragma warning restore 612, 618
        }
    }
}
