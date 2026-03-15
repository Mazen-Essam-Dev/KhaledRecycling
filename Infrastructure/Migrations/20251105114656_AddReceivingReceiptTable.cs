using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceivingReceiptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceivingReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Received = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Payment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SumOfAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Being = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpenseId = table.Column<int>(type: "int", nullable: true),
                    ManagerSignitureId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceivingReceipts_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceivingReceipts_Signatures_ManagerSignitureId",
                        column: x => x.ManagerSignitureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingReceipts_ExpenseId",
                table: "ReceivingReceipts",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingReceipts_ManagerSignitureId",
                table: "ReceivingReceipts",
                column: "ManagerSignitureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceivingReceipts");
        }
    }
}
