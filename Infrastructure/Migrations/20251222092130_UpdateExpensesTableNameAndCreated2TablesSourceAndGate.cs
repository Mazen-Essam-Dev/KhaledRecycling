using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpensesTableNameAndCreated2TablesSourceAndGate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReceipts_Expenses_ExpenseId",
                table: "ReceivingReceipts");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.CreateTable(
                name: "ExpensesGate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesGate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpensesSource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseAndReceiptAndOther",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemType = table.Column<int>(type: "int", nullable: true),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetItemId = table.Column<int>(type: "int", nullable: true),
                    ExpensesGateId = table.Column<int>(type: "int", nullable: true),
                    ExpensesSourceId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ExpensesReportId = table.Column<int>(type: "int", nullable: true),
                    ExpenseAndReceiptReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseAndReceiptAndOther", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalTable: "BudgetItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_ExpenseAndReceiptReports_ExpenseAndReceiptReportId",
                        column: x => x.ExpenseAndReceiptReportId,
                        principalTable: "ExpenseAndReceiptReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_ExpensesGate_ExpensesGateId",
                        column: x => x.ExpensesGateId,
                        principalTable: "ExpensesGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_ExpensesReports_ExpensesReportId",
                        column: x => x.ExpensesReportId,
                        principalTable: "ExpensesReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_ExpensesSource_ExpensesSourceId",
                        column: x => x.ExpensesSourceId,
                        principalTable: "ExpensesSource",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseAndReceiptAndOther_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_BudgetItemId",
                table: "ExpenseAndReceiptAndOther",
                column: "BudgetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_ExpenseAndReceiptReportId",
                table: "ExpenseAndReceiptAndOther",
                column: "ExpenseAndReceiptReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_ExpensesGateId",
                table: "ExpenseAndReceiptAndOther",
                column: "ExpensesGateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_ExpensesReportId",
                table: "ExpenseAndReceiptAndOther",
                column: "ExpensesReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_ExpensesSourceId",
                table: "ExpenseAndReceiptAndOther",
                column: "ExpensesSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_SupplierId",
                table: "ExpenseAndReceiptAndOther",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts",
                column: "ExpenseId",
                principalTable: "ExpenseAndReceiptAndOther",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts");

            migrationBuilder.DropTable(
                name: "ExpenseAndReceiptAndOther");

            migrationBuilder.DropTable(
                name: "ExpensesGate");

            migrationBuilder.DropTable(
                name: "ExpensesSource");

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetItemId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpenseAndReceiptReportId = table.Column<int>(type: "int", nullable: true),
                    ExpensesGateId = table.Column<int>(type: "int", nullable: true),
                    ExpensesReportId = table.Column<int>(type: "int", nullable: true),
                    ExpensesSourceId = table.Column<int>(type: "int", nullable: true),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalTable: "BudgetItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseAndReceiptReports_ExpenseAndReceiptReportId",
                        column: x => x.ExpenseAndReceiptReportId,
                        principalTable: "ExpenseAndReceiptReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_ExpensesReports_ExpensesReportId",
                        column: x => x.ExpensesReportId,
                        principalTable: "ExpensesReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BudgetItemId",
                table: "Expenses",
                column: "BudgetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseAndReceiptReportId",
                table: "Expenses",
                column: "ExpenseAndReceiptReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpensesReportId",
                table: "Expenses",
                column: "ExpensesReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SupplierId",
                table: "Expenses",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReceipts_Expenses_ExpenseId",
                table: "ReceivingReceipts",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }
    }
}
