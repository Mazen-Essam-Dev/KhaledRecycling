using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedStoreNotesIn4Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreNotes",
                table: "OrderSellToFactories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreNotes",
                table: "OrderSellToClients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreNotes",
                table: "OrderBuyFromFactories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreNotes",
                table: "OrderBuyFromClients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreNotes",
                table: "OrderSellToFactories");

            migrationBuilder.DropColumn(
                name: "StoreNotes",
                table: "OrderSellToClients");

            migrationBuilder.DropColumn(
                name: "StoreNotes",
                table: "OrderBuyFromFactories");

            migrationBuilder.DropColumn(
                name: "StoreNotes",
                table: "OrderBuyFromClients");
        }
    }
}
