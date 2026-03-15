using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameAr",
                value: "مصروفات");

            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameAr",
                value: "مقبوضات");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameAr",
                value: "Expenses");

            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameAr",
                value: "Receipts");
        }
    }
}
