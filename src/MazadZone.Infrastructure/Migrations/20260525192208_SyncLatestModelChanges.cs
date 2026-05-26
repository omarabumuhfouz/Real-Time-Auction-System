using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncLatestModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "ItemImages",
                newName: "IsMain");

            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "DisputeImages",
                newName: "IsMain");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "ItemImages",
                newName: "isMain");

            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "DisputeImages",
                newName: "isMain");
        }
    }
}
