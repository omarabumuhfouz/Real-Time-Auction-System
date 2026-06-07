using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeparateAddressesAsSeparatedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Building",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "Landmark",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Bidders");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "PaymentMethods",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiptAddress_IsDefault",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShippingAddress_IsDefault",
                table: "Auctions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BidderAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Building = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Landmark = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    BidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidderAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidderAddresses_Bidders_BidderId",
                        column: x => x.BidderId,
                        principalTable: "Bidders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UserId1",
                table: "PaymentMethods",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_BidderAddresses_BidderId",
                table: "BidderAddresses",
                column: "BidderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Users_UserId1",
                table: "PaymentMethods",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Users_UserId1",
                table: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "BidderAddresses");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_UserId1",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "ReceiptAddress_IsDefault",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_IsDefault",
                table: "Auctions");

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Bidders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Bidders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Landmark",
                table: "Bidders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Bidders",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
