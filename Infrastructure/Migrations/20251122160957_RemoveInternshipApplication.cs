using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInternshipApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipApplications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternshipApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: true),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    EmployeeId1 = table.Column<int>(type: "integer", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    IsVolunteer = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TaskSuccessPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipApplications", x => x.Id);
                    table.CheckConstraint("CK_InternshipApplications_TaskSuccessRange", "\"TaskSuccessPercent\" >= 0 AND \"TaskSuccessPercent\" <= 100");
                    table.ForeignKey(
                        name: "FK_InternshipApplications_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipApplications_Employess_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InternshipApplications_Employess_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "Employess",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InternshipApplications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_DepartmentId",
                table: "InternshipApplications",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_EmployeeId",
                table: "InternshipApplications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_EmployeeId1",
                table: "InternshipApplications",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_StudentId",
                table: "InternshipApplications",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_StudentId_Role",
                table: "InternshipApplications",
                columns: new[] { "StudentId", "Role" });
        }
    }
}
