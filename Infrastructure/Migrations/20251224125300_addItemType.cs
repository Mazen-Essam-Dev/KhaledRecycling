using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "Expenses", "Expenses" },
                    { 2, "Receipts", "Receipts" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAndReceiptAndOther_ItemType",
                table: "ExpenseAndReceiptAndOther",
                column: "ItemType");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseAndReceiptAndOther_ItemTypes_ItemType",
                table: "ExpenseAndReceiptAndOther",
                column: "ItemType",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseAndReceiptAndOther_ItemTypes_ItemType",
                table: "ExpenseAndReceiptAndOther");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseAndReceiptAndOther_ItemType",
                table: "ExpenseAndReceiptAndOther");
        }
    }
}
