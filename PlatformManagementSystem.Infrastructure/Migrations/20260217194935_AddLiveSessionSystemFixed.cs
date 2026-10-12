using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveSessionSystemFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "LiveSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "LiveSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "LiveSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LiveSessionAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiveSessionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveSessionAttendances_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LiveSessionAttendances_LiveSessions_LiveSessionId",
                        column: x => x.LiveSessionId,
                        principalTable: "LiveSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_InstructorId",
                table: "LiveSessions",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionAttendances_LiveSessionId",
                table: "LiveSessionAttendances",
                column: "LiveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionAttendances_StudentId",
                table: "LiveSessionAttendances",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveSessions_AspNetUsers_InstructorId",
                table: "LiveSessions",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveSessions_AspNetUsers_InstructorId",
                table: "LiveSessions");

            migrationBuilder.DropTable(
                name: "LiveSessionAttendances");

            migrationBuilder.DropIndex(
                name: "IX_LiveSessions_InstructorId",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "LiveSessions");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "LiveSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
