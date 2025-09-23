using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DsiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateWorksConducted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMainImage",
                table: "WorksConducteds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 8, 11, 9, 8, 52, 523, DateTimeKind.Local).AddTicks(2200), new Guid("82681950-d30e-493e-8e2a-3662237f2137") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMainImage",
                table: "WorksConducteds");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 8, 7, 10, 57, 28, 622, DateTimeKind.Local).AddTicks(3285), new Guid("0ade71b4-0b5d-4b13-aea7-57b6ad38b9d4") });
        }
    }
}
