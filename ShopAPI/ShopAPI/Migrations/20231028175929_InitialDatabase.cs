using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShopAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.CartId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    FormFactor = table.Column<string>(type: "text", nullable: false),
                    SidePanel = table.Column<string>(type: "text", nullable: false),
                    PowerSupply = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cases_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CpuCoolers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    IsWaterCooled = table.Column<bool>(type: "boolean", nullable: false),
                    Size = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuCoolers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CpuCoolers_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cpus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    Cores = table.Column<int>(type: "integer", nullable: false),
                    Series = table.Column<string>(type: "text", nullable: false),
                    IntegratedGraphics = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cpus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cpus_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    MemoryType = table.Column<string>(type: "text", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memory_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    Chipset = table.Column<string>(type: "text", nullable: false),
                    MemoryType = table.Column<string>(type: "text", nullable: false),
                    FormFactor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Motherboards_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerSupplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Wattage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSupplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerSupplies_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ConnectionType = table.Column<string>(type: "text", nullable: false),
                    DriveType = table.Column<string>(type: "text", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Storage_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Series = table.Column<string>(type: "text", nullable: false),
                    ClockSpeed = table.Column<int>(type: "integer", nullable: false),
                    VramSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoCards_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "CpuCoolers");

            migrationBuilder.DropTable(
                name: "Cpus");

            migrationBuilder.DropTable(
                name: "Memory");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "PowerSupplies");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "VideoCards");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
