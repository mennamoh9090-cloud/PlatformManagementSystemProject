using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_ExamId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_ExamId1",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExams_Quizzes_ExamId",
                table: "StudentExams");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_AspNetUsers_StudentId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ExamId1",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ExamId1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Enrollments");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "Exam");

            migrationBuilder.RenameIndex(
                name: "IX_Quizzes_CourseId",
                table: "Exam",
                newName: "IX_Exam_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "MeetingUrl",
                table: "LiveSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exam",
                table: "Exam",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLessonProgresses_CourseId",
                table: "StudentLessonProgresses",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_Courses_CourseId",
                table: "Exam",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exam_ExamId",
                table: "Questions",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExams_Exam_ExamId",
                table: "StudentExams",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_AspNetUsers_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Courses_CourseId",
                table: "StudentLessonProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Courses_CourseId",
                table: "Exam");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exam_ExamId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExams_Exam_ExamId",
                table: "StudentExams");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_AspNetUsers_StudentId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Courses_CourseId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentLessonProgresses_CourseId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exam",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "MeetingUrl",
                table: "LiveSessions");

            migrationBuilder.RenameTable(
                name: "Exam",
                newName: "Quizzes");

            migrationBuilder.RenameIndex(
                name: "IX_Exam_CourseId",
                table: "Quizzes",
                newName: "IX_Quizzes_CourseId");

            migrationBuilder.AddColumn<int>(
                name: "ExamId1",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Enrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamId1",
                table: "Questions",
                column: "ExamId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_ExamId",
                table: "Questions",
                column: "ExamId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_ExamId1",
                table: "Questions",
                column: "ExamId1",
                principalTable: "Quizzes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExams_Quizzes_ExamId",
                table: "StudentExams",
                column: "ExamId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_AspNetUsers_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
