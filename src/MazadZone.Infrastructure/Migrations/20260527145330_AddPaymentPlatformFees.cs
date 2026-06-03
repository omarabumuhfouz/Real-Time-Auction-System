using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentPlatformFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GrossAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GrossCurrency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NetCurrency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFee",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PlatformFeeCurrency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "GrossCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NetCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PlatformFee",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PlatformFeeCurrency",
                table: "Payments");
        }
    }
}
