using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveBidderVerificationToSeparateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create the new BidderVerifications table
            migrationBuilder.CreateTable(
                name: "BidderVerifications",
                columns: table => new
                {
                    BidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExtractedFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidderVerifications", x => x.BidderId);
                    table.ForeignKey(
                        name: "FK_BidderVerifications_Bidders_BidderId",
                        column: x => x.BidderId,
                        principalTable: "Bidders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Step 2: Migrate existing data from Bidders to BidderVerifications
            migrationBuilder.Sql(@"
                INSERT INTO BidderVerifications (BidderId, NationalId, IsVerified, Status, ExtractedFullName, RejectionReason)
                SELECT Id, NationalId, IsVerified, VerificationStatus, ExtractedFullName, VerificationRejectionReason
                FROM Bidders
                WHERE NationalId IS NOT NULL;
            ");

            // Step 3: Drop old verification columns from Bidders
            migrationBuilder.DropColumn(
                name: "ExtractedFullName",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "VerificationRejectionReason",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Bidders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidderVerifications");

            migrationBuilder.AddColumn<string>(
                name: "ExtractedFullName",
                table: "Bidders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Bidders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Bidders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationRejectionReason",
                table: "Bidders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "Bidders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
