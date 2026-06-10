using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFKUserTypeIn4TablesSeelAndBuy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FKUserType",
                table: "OrderSellToFactories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FKUserType",
                table: "OrderSellToClients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FKUserType",
                table: "OrderBuyFromFactories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FKUserType",
                table: "OrderBuyFromClients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToFactories_FKUserType",
                table: "OrderSellToFactories",
                column: "FKUserType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToClients_FKUserType",
                table: "OrderSellToClients",
                column: "FKUserType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromFactories_FKUserType",
                table: "OrderBuyFromFactories",
                column: "FKUserType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromClients_FKUserType",
                table: "OrderBuyFromClients",
                column: "FKUserType");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromClients_UserTypes_FKUserType",
                table: "OrderBuyFromClients",
                column: "FKUserType",
                principalTable: "UserTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromFactories_UserTypes_FKUserType",
                table: "OrderBuyFromFactories",
                column: "FKUserType",
                principalTable: "UserTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToClients_UserTypes_FKUserType",
                table: "OrderSellToClients",
                column: "FKUserType",
                principalTable: "UserTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToFactories_UserTypes_FKUserType",
                table: "OrderSellToFactories",
                column: "FKUserType",
                principalTable: "UserTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBuyFromClients_UserTypes_FKUserType",
                table: "OrderBuyFromClients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderBuyFromFactories_UserTypes_FKUserType",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSellToClients_UserTypes_FKUserType",
                table: "OrderSellToClients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSellToFactories_UserTypes_FKUserType",
                table: "OrderSellToFactories");

            migrationBuilder.DropIndex(
                name: "IX_OrderSellToFactories_FKUserType",
                table: "OrderSellToFactories");

            migrationBuilder.DropIndex(
                name: "IX_OrderSellToClients_FKUserType",
                table: "OrderSellToClients");

            migrationBuilder.DropIndex(
                name: "IX_OrderBuyFromFactories_FKUserType",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropIndex(
                name: "IX_OrderBuyFromClients_FKUserType",
                table: "OrderBuyFromClients");

            migrationBuilder.DropColumn(
                name: "FKUserType",
                table: "OrderSellToFactories");

            migrationBuilder.DropColumn(
                name: "FKUserType",
                table: "OrderSellToClients");

            migrationBuilder.DropColumn(
                name: "FKUserType",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropColumn(
                name: "FKUserType",
                table: "OrderBuyFromClients");
        }
    }
}
