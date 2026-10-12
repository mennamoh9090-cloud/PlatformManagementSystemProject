using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWhiteboardPersist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WhiteboardEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WhiteboardEvents");
        }
    }
}
