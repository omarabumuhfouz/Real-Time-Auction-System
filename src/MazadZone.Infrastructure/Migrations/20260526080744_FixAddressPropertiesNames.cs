using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAddressPropertiesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptAddress_Street",
                table: "Orders",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "ReceiptAddress_Landmark",
                table: "Orders",
                newName: "Landmark");

            migrationBuilder.RenameColumn(
                name: "ReceiptAddress_City",
                table: "Orders",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "ReceiptAddress_Building",
                table: "Orders",
                newName: "Building");

            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "ItemImages",
                newName: "IsMain");

            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "DisputeImages",
                newName: "IsMain");

            migrationBuilder.RenameColumn(
                name: "DefaultShippingAddress_Street",
                table: "Bidders",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "DefaultShippingAddress_Landmark",
                table: "Bidders",
                newName: "Landmark");

            migrationBuilder.RenameColumn(
                name: "DefaultShippingAddress_City",
                table: "Bidders",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "DefaultShippingAddress_Building",
                table: "Bidders",
                newName: "Building");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Street",
                table: "Auctions",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Landmark",
                table: "Auctions",
                newName: "Landmark");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_City",
                table: "Auctions",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Building",
                table: "Auctions",
                newName: "Building");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Orders",
                newName: "ReceiptAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Landmark",
                table: "Orders",
                newName: "ReceiptAddress_Landmark");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Orders",
                newName: "ReceiptAddress_City");

            migrationBuilder.RenameColumn(
                name: "Building",
                table: "Orders",
                newName: "ReceiptAddress_Building");

            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "ItemImages",
                newName: "isMain");

            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "DisputeImages",
                newName: "isMain");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Bidders",
                newName: "DefaultShippingAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Landmark",
                table: "Bidders",
                newName: "DefaultShippingAddress_Landmark");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Bidders",
                newName: "DefaultShippingAddress_City");

            migrationBuilder.RenameColumn(
                name: "Building",
                table: "Bidders",
                newName: "DefaultShippingAddress_Building");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Auctions",
                newName: "ShippingAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Landmark",
                table: "Auctions",
                newName: "ShippingAddress_Landmark");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Auctions",
                newName: "ShippingAddress_City");

            migrationBuilder.RenameColumn(
                name: "Building",
                table: "Auctions",
                newName: "ShippingAddress_Building");
        }
    }
}
