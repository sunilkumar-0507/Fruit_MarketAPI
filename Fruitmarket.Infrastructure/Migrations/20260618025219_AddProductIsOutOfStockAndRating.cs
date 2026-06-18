using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fruitmarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIsOutOfStockAndRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOutOfStock",
                table: "Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Products",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "IsOutOfStock", "Rating" },
                values: new object[] { false, 0.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "IsOutOfStock", "Rating" },
                values: new object[] { false, 0.0 });

            // Backfill Rating for any products that already have reviews (no-op on an empty Reviews table).
            migrationBuilder.Sql(
                "UPDATE `Products` p SET p.`Rating` = COALESCE((SELECT AVG(r.`Rating`) FROM `Reviews` r WHERE r.`ProductId` = p.`Id`), 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOutOfStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Products");
        }
    }
}
