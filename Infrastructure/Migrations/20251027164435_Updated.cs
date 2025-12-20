using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipTasks");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Universities");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Universities",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Universities",
                keyColumn: "Id",
                keyValue: 1,
                column: "City",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Universities");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Universities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InternshipTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternshipApplicationId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SuccessPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipTasks", x => x.Id);
                    table.CheckConstraint("CK_InternshipTasks_SuccessRange", "\"SuccessPercent\" >= 0 AND \"SuccessPercent\" <= 100");
                    table.ForeignKey(
                        name: "FK_InternshipTasks_InternshipApplications_InternshipApplicatio~",
                        column: x => x.InternshipApplicationId,
                        principalTable: "InternshipApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Universities",
                keyColumn: "Id",
                keyValue: 1,
                column: "Country",
                value: "Turkey");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipTasks_InternshipApplicationId",
                table: "InternshipTasks",
                column: "InternshipApplicationId");
        }
    }
}
