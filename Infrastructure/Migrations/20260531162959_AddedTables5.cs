using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTables5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoneyPusheds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Money = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TypeTransaction = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyPusheds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderBuyFromFactoryAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OrderBuyFromFactoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBuyFromFactoryAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderSellToClientAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OrderSellToClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSellToClientAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderSellToFactoryAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OrderSellToFactoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSellToFactoryAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPointss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalsReNew = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Totals = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPointss", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoneyPusheds");

            migrationBuilder.DropTable(
                name: "OrderBuyFromFactoryAttachments");

            migrationBuilder.DropTable(
                name: "OrderSellToClientAttachments");

            migrationBuilder.DropTable(
                name: "OrderSellToFactoryAttachments");

            migrationBuilder.DropTable(
                name: "UserPointss");
        }
    }
}
