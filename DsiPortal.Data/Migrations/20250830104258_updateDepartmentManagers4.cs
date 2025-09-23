using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DsiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDepartmentManagers4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalNo",
                table: "DepartmentManagers");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "DepartmentManagers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 42, 57, 371, DateTimeKind.Local).AddTicks(9828), new Guid("775034d4-50f0-4ced-901b-bcf265405622") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "DepartmentManagers");

            migrationBuilder.AddColumn<int>(
                name: "InternalNo",
                table: "DepartmentManagers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 17, 41, 524, DateTimeKind.Local).AddTicks(2777), new Guid("579b6502-d50a-4f51-9382-d910c0efaa31") });
        }
    }
}
