using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReportType4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportType_ReportTypeId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType");

            migrationBuilder.RenameTable(
                name: "ReportType",
                newName: "ReportTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportTypes_ReportTypeId",
                table: "ExpensesAndReciptReportSigns",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportTypes_ReportTypeId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes");

            migrationBuilder.RenameTable(
                name: "ReportTypes",
                newName: "ReportType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportType_ReportTypeId",
                table: "ExpensesAndReciptReportSigns",
                column: "ReportTypeId",
                principalTable: "ReportType",
                principalColumn: "Id");
        }
    }
}
