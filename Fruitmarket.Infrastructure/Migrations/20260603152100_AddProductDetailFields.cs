using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fruitmarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutEn",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AboutTa",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BenefitsEn",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BenefitsTa",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsageEn",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsageTa",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "AboutEn", "AboutTa", "BenefitsEn", "BenefitsTa", "UsageEn", "UsageTa" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "AboutEn", "AboutTa", "BenefitsEn", "BenefitsTa", "UsageEn", "UsageTa" },
                values: new object[] { null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AboutTa",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BenefitsEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BenefitsTa",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UsageEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UsageTa",
                table: "Products");
        }
    }
}
