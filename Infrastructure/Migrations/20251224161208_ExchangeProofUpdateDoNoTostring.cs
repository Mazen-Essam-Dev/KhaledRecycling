using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExchangeProofUpdateDoNoTostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentNo",
                table: "ExchangeProofs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
            migrationBuilder.AlterColumn<int>(
                name: "DocumentNo",
                table: "ExchangeProofs",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
