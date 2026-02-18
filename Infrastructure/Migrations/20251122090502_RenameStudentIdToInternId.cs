using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStudentIdToInternId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Students_StudentId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Assignments",
                newName: "InternId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_StudentId",
                table: "Assignments",
                newName: "IX_Assignments_InternId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Students_InternId",
                table: "Assignments",
                column: "InternId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Students_InternId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "InternId",
                table: "Assignments",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_InternId",
                table: "Assignments",
                newName: "IX_Assignments_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Students_StudentId",
                table: "Assignments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
