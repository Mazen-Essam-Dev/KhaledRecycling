using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeddingReportSalaryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReportSalaryTypes",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "تقرير الرواتب والاجور", "Salary Report" },
                    { 2, "تقرير الخصومات والعلاوات", "Discounts and Bonuses Report" },
                    { 3, "تقرير الغيابات", "Absences Report" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportSalaryTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReportSalaryTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReportSalaryTypes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
