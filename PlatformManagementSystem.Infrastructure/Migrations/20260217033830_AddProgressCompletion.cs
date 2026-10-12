using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Enrollments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Enrollments");
        }
    }
}
