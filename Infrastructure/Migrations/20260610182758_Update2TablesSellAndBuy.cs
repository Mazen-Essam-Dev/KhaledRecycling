using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update2TablesSellAndBuy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBuyFromFactories_SubWastes_FKSubWasteId",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSellToClients_SubWastes_FKSubWasteId",
                table: "OrderSellToClients");

            migrationBuilder.RenameColumn(
                name: "FKSubWasteId",
                table: "OrderSellToClients",
                newName: "FKSubProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderSellToClients_FKSubWasteId",
                table: "OrderSellToClients",
                newName: "IX_OrderSellToClients_FKSubProductId");

            migrationBuilder.RenameColumn(
                name: "FKSubWasteId",
                table: "OrderBuyFromFactories",
                newName: "FKSubProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderBuyFromFactories_FKSubWasteId",
                table: "OrderBuyFromFactories",
                newName: "IX_OrderBuyFromFactories_FKSubProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromFactories_SubProducts_FKSubProductId",
                table: "OrderBuyFromFactories",
                column: "FKSubProductId",
                principalTable: "SubProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToClients_SubProducts_FKSubProductId",
                table: "OrderSellToClients",
                column: "FKSubProductId",
                principalTable: "SubProducts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBuyFromFactories_SubProducts_FKSubProductId",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSellToClients_SubProducts_FKSubProductId",
                table: "OrderSellToClients");

            migrationBuilder.RenameColumn(
                name: "FKSubProductId",
                table: "OrderSellToClients",
                newName: "FKSubWasteId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderSellToClients_FKSubProductId",
                table: "OrderSellToClients",
                newName: "IX_OrderSellToClients_FKSubWasteId");

            migrationBuilder.RenameColumn(
                name: "FKSubProductId",
                table: "OrderBuyFromFactories",
                newName: "FKSubWasteId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderBuyFromFactories_FKSubProductId",
                table: "OrderBuyFromFactories",
                newName: "IX_OrderBuyFromFactories_FKSubWasteId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromFactories_SubWastes_FKSubWasteId",
                table: "OrderBuyFromFactories",
                column: "FKSubWasteId",
                principalTable: "SubWastes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToClients_SubWastes_FKSubWasteId",
                table: "OrderSellToClients",
                column: "FKSubWasteId",
                principalTable: "SubWastes",
                principalColumn: "Id");
        }
    }
}
