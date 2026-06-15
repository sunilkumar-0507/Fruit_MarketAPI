using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fruitmarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmersAndBaskets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Images = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Items = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Farmers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Village = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Produce = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeeklySupplyKg = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<double>(type: "double", nullable: true),
                    Phone = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Baskets",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "Images", "IsActive", "IsDeleted", "Items", "Name", "Price", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("ba000001-0000-0000-0000-000000000001"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Handcrafted festival hamper with premium mangoes, bananas, and pomegranates.", "[\"/images/categories/fruit-baskets.jpg\",\"/images/products/mangoes.jpeg\",\"/images/categories/p-pomegranate.jpg\"]", true, false, "Mango × 4, Banana × 6, Pomegranate × 2", "Pongal Festival Basket", 1450m, null },
                    { new Guid("ba000002-0000-0000-0000-000000000002"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Imported tropical selection — rambutan, dragon fruit, and mangosteen.", "[\"/images/products/wa2-rambutan.jpeg\",\"/images/products/wa2-dragon-fruit.jpeg\",\"/images/products/wa2-mangosteen.jpeg\",\"/images/categories/p-pomegranate.jpg\"]", true, false, "Rambutan × 6, Dragon Fruit × 2, Mangosteen × 3", "Exotic Mix Combo", 850m, null }
                });

            migrationBuilder.InsertData(
                table: "Farmers",
                columns: new[] { "Id", "CreatedAtUtc", "IsActive", "IsDeleted", "Name", "Phone", "Produce", "Rating", "UpdatedAtUtc", "Village", "WeeklySupplyKg" },
                values: new object[,]
                {
                    { new Guid("fa000001-0000-0000-0000-000000000001"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Murugesan P.", "+91 94433 00001", "Mango, Banana", 4.9000000000000004, null, "Tenkasi", 800 },
                    { new Guid("fa000002-0000-0000-0000-000000000002"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Rajan T.", "+91 94433 00002", "Guava, Papaya", 4.7000000000000002, null, "Courtallam", 450 },
                    { new Guid("fa000003-0000-0000-0000-000000000003"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Selvam K.", "+91 94433 00003", "Banana, Jackfruit", 4.7999999999999998, null, "Alangulam", 600 },
                    { new Guid("fa000004-0000-0000-0000-000000000004"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), false, false, "Lakshmi A.", "+91 94433 00004", "Pomegranate, Grapes", 4.5999999999999996, null, "Kadayanallur", 300 },
                    { new Guid("fa000005-0000-0000-0000-000000000005"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Kumar M.", "+91 94433 00005", "Watermelon, Pineapple", 4.5, null, "Sankarankovil", 500 },
                    { new Guid("fa000006-0000-0000-0000-000000000006"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Pandian S.", "+91 94433 00006", "Dry Fruits, Seasonal", 4.7000000000000002, null, "Tirunelveli", 250 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "Farmers");
        }
    }
}
