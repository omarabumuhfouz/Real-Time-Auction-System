using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNationalIdFromSeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Sellers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Sellers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
