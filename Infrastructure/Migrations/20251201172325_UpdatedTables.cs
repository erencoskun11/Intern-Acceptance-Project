using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Employess",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AppUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employess_AppUserId",
                table: "Employess",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employess_AppUsers_AppUserId",
                table: "Employess",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employess_AppUsers_AppUserId",
                table: "Employess");

            migrationBuilder.DropIndex(
                name: "IX_Employess_AppUserId",
                table: "Employess");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Employess");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AppUsers");
        }
    }
}
