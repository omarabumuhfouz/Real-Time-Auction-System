using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEnforcementAndLastLoginAndAndDisputeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Orders_OrderId",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_OrderId",
                table: "Disputes");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Disputes",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "EnforcementReason",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisputeId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisputeTypeId",
                table: "Disputes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Disputes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DisputeImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isMain = table.Column<bool>(type: "bit", nullable: false),
                    DisputeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisputeImages_Disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "Disputes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisputeTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_DisputeTypeId",
                table: "Disputes",
                column: "DisputeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeImages_DisputeId",
                table: "DisputeImages",
                column: "DisputeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_DisputeTypes_DisputeTypeId",
                table: "Disputes",
                column: "DisputeTypeId",
                principalTable: "DisputeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_DisputeTypes_DisputeTypeId",
                table: "Disputes");

            migrationBuilder.DropTable(
                name: "DisputeImages");

            migrationBuilder.DropTable(
                name: "DisputeTypes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_DisputeTypeId",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "EnforcementReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisputeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DisputeTypeId",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Disputes");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Disputes",
                newName: "Reason");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_OrderId",
                table: "Disputes",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Orders_OrderId",
                table: "Disputes",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
