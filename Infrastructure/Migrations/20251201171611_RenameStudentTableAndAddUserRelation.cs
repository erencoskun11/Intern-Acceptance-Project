using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStudentTableAndAddUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Students_InternId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Interns");

            migrationBuilder.RenameIndex(
                name: "IX_Students_UniversityId",
                table: "Interns",
                newName: "IX_Interns_UniversityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interns",
                table: "Interns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Interns_InternId",
                table: "Assignments",
                column: "InternId",
                principalTable: "Interns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interns_Universities_UniversityId",
                table: "Interns",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Interns_InternId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Interns_Universities_UniversityId",
                table: "Interns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interns",
                table: "Interns");

            migrationBuilder.RenameTable(
                name: "Interns",
                newName: "Students");

            migrationBuilder.RenameIndex(
                name: "IX_Interns_UniversityId",
                table: "Students",
                newName: "IX_Students_UniversityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Students_InternId",
                table: "Assignments",
                column: "InternId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
