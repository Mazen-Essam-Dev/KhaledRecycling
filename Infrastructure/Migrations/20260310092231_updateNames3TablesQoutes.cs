using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNames3TablesQoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quotesItems_quotes_quotesId",
                table: "quotesItems");

            migrationBuilder.RenameColumn(
                name: "quotesId",
                table: "quotesItems",
                newName: "quoteId");

            migrationBuilder.RenameColumn(
                name: "quotesItemId",
                table: "quotesItems",
                newName: "quoteItemId");

            migrationBuilder.RenameIndex(
                name: "IX_quotesItems_quotesId",
                table: "quotesItems",
                newName: "IX_quotesItems_quoteId");

            migrationBuilder.RenameColumn(
                name: "quotesCode",
                table: "quotes",
                newName: "quoteCode");

            migrationBuilder.AddForeignKey(
                name: "FK_quotesItems_quotes_quoteId",
                table: "quotesItems",
                column: "quoteId",
                principalTable: "quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quotesItems_quotes_quoteId",
                table: "quotesItems");

            migrationBuilder.RenameColumn(
                name: "quoteId",
                table: "quotesItems",
                newName: "quotesId");

            migrationBuilder.RenameColumn(
                name: "quoteItemId",
                table: "quotesItems",
                newName: "quotesItemId");

            migrationBuilder.RenameIndex(
                name: "IX_quotesItems_quoteId",
                table: "quotesItems",
                newName: "IX_quotesItems_quotesId");

            migrationBuilder.RenameColumn(
                name: "quoteCode",
                table: "quotes",
                newName: "quotesCode");

            migrationBuilder.AddForeignKey(
                name: "FK_quotesItems_quotes_quotesId",
                table: "quotesItems",
                column: "quotesId",
                principalTable: "quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
