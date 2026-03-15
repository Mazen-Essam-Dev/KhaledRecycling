using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mergeExpenseAndReciptsInOneTableExpenses2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Expenses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpenseAndReceiptReportId = table.Column<int>(type: "int", nullable: true),
                    ExpensesSourceId = table.Column<int>(type: "int", nullable: true),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_ExpenseAndReceiptReports_ExpenseAndReceiptReportId",
                        column: x => x.ExpenseAndReceiptReportId,
                        principalTable: "ExpenseAndReceiptReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ExpenseAndReceiptReportId",
                table: "Receipts",
                column: "ExpenseAndReceiptReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_SupplierId",
                table: "Receipts",
                column: "SupplierId");
        }
    }
}
