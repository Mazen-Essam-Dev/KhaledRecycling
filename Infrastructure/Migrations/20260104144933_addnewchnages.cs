using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addnewchnages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReportTypes",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, " مصروفات نثرية", "Expenses" },
                    { 2, "مقبوضات ومدفوعات", "Expenses And Receipts" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
