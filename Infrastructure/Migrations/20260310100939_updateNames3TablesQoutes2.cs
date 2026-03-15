using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNames3TablesQoutes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "quoteIsConfirmed",
                table: "quotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ItemSupplierIsConfirmed",
                table: "ItemSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quoteIsConfirmed",
                table: "quotes");

            migrationBuilder.DropColumn(
                name: "ItemSupplierIsConfirmed",
                table: "ItemSuppliers");
        }
    }
}
