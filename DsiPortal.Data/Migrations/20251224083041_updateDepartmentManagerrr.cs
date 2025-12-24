using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DsiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDepartmentManagerrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "DepartmentManagers");

            migrationBuilder.AddColumn<int>(
                name: "TitleId",
                table: "DepartmentManagers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 12, 24, 11, 30, 39, 304, DateTimeKind.Local).AddTicks(7018), new Guid("2a94c1f3-d799-42f8-88b9-2df93c9a4046") });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentManagers_TitleId",
                table: "DepartmentManagers",
                column: "TitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentManagers_Titles_TitleId",
                table: "DepartmentManagers",
                column: "TitleId",
                principalTable: "Titles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentManagers_Titles_TitleId",
                table: "DepartmentManagers");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentManagers_TitleId",
                table: "DepartmentManagers");

            migrationBuilder.DropColumn(
                name: "TitleId",
                table: "DepartmentManagers");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "DepartmentManagers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 11, 10, 18, 26, 9, 958, DateTimeKind.Local).AddTicks(1315), new Guid("63dc49f4-3e2b-4e89-8d96-f0694af7ab5b") });
        }
    }
}
