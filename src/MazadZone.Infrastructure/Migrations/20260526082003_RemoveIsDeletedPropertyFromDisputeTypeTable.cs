using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsDeletedPropertyFromDisputeTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DisputeTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DisputeTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
