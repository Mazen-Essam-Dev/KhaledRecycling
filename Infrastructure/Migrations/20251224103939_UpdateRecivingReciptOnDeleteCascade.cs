using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecivingReciptOnDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts",
                column: "ExpenseId",
                principalTable: "ExpenseAndReceiptAndOther",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReceipts_ExpenseAndReceiptAndOther_ExpenseId",
                table: "ReceivingReceipts",
                column: "ExpenseId",
                principalTable: "ExpenseAndReceiptAndOther",
                principalColumn: "Id");
        }
    }
}
