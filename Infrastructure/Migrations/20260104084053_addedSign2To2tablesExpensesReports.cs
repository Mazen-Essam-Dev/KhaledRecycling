using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedSign2To2tablesExpensesReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcountantSignatureId",
                table: "ExpensesReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerSignitureId",
                table: "ExpensesReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcountantSignatureId",
                table: "ExpenseAndReceiptReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerSignitureId",
                table: "ExpenseAndReceiptReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpensesAndReciptReportSigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeTable = table.Column<int>(type: "int", nullable: true),
                    ItemTypeEntityId = table.Column<int>(type: "int", nullable: true),
                    ExpensesSourceId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    AcountantSignatureId = table.Column<int>(type: "int", nullable: true),
                    ManagerSignitureId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesAndReciptReportSigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpensesAndReciptReportSigns_ExpensesSource_ExpensesSourceId",
                        column: x => x.ExpensesSourceId,
                        principalTable: "ExpensesSource",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpensesAndReciptReportSigns_ItemTypes_ItemTypeEntityId",
                        column: x => x.ItemTypeEntityId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpensesAndReciptReportSigns_Signatures_AcountantSignatureId",
                        column: x => x.AcountantSignatureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpensesAndReciptReportSigns_Signatures_ManagerSignitureId",
                        column: x => x.ManagerSignitureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesReports_AcountantSignatureId",
                table: "ExpensesReports",
                column: "AcountantSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesReports_ManagerSignitureId",
                table: "ExpensesReports",
                column: "ManagerSignitureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptReports_AcountantSignatureId",
                table: "ExpenseAndReceiptReports",
                column: "AcountantSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptReports_ManagerSignitureId",
                table: "ExpenseAndReceiptReports",
                column: "ManagerSignitureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_AcountantSignatureId",
                table: "ExpensesAndReciptReportSigns",
                column: "AcountantSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns",
                column: "ExpensesSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns",
                column: "ItemTypeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ManagerSignitureId",
                table: "ExpensesAndReciptReportSigns",
                column: "ManagerSignitureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseAndReceiptReports_Signatures_AcountantSignatureId",
                table: "ExpenseAndReceiptReports",
                column: "AcountantSignatureId",
                principalTable: "Signatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseAndReceiptReports_Signatures_ManagerSignitureId",
                table: "ExpenseAndReceiptReports",
                column: "ManagerSignitureId",
                principalTable: "Signatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesReports_Signatures_AcountantSignatureId",
                table: "ExpensesReports",
                column: "AcountantSignatureId",
                principalTable: "Signatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesReports_Signatures_ManagerSignitureId",
                table: "ExpensesReports",
                column: "ManagerSignitureId",
                principalTable: "Signatures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseAndReceiptReports_Signatures_AcountantSignatureId",
                table: "ExpenseAndReceiptReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseAndReceiptReports_Signatures_ManagerSignitureId",
                table: "ExpenseAndReceiptReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesReports_Signatures_AcountantSignatureId",
                table: "ExpensesReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesReports_Signatures_ManagerSignitureId",
                table: "ExpensesReports");

            migrationBuilder.DropTable(
                name: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropIndex(
                name: "IX_ExpensesReports_AcountantSignatureId",
                table: "ExpensesReports");

            migrationBuilder.DropIndex(
                name: "IX_ExpensesReports_ManagerSignitureId",
                table: "ExpensesReports");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseAndReceiptReports_AcountantSignatureId",
                table: "ExpenseAndReceiptReports");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseAndReceiptReports_ManagerSignitureId",
                table: "ExpenseAndReceiptReports");

            migrationBuilder.DropColumn(
                name: "AcountantSignatureId",
                table: "ExpensesReports");

            migrationBuilder.DropColumn(
                name: "ManagerSignitureId",
                table: "ExpensesReports");

            migrationBuilder.DropColumn(
                name: "AcountantSignatureId",
                table: "ExpenseAndReceiptReports");

            migrationBuilder.DropColumn(
                name: "ManagerSignitureId",
                table: "ExpenseAndReceiptReports");
        }
    }
}
