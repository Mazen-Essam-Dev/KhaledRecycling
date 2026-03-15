using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added3TablesForQuotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quotesCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextArea = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    OrderTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SignatureSuperVisorId = table.Column<int>(type: "int", nullable: true),
                    SignatureAccountantId = table.Column<int>(type: "int", nullable: true),
                    SignatureUserSecetaryId = table.Column<int>(type: "int", nullable: true),
                    SignatureManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quotes_Signatures_SignatureAccountantId",
                        column: x => x.SignatureAccountantId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_quotes_Signatures_SignatureManagerId",
                        column: x => x.SignatureManagerId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_quotes_Signatures_SignatureSuperVisorId",
                        column: x => x.SignatureSuperVisorId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_quotes_Signatures_SignatureUserSecetaryId",
                        column: x => x.SignatureUserSecetaryId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "quotesItems",
                columns: table => new
                {
                    quotesItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quotesId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotesItems", x => x.quotesItemId);
                    table.ForeignKey(
                        name: "FK_quotesItems_quotes_quotesId",
                        column: x => x.quotesId,
                        principalTable: "quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemSuppliers",
                columns: table => new
                {
                    ItemSupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quotesItemId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SinglePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSuppliers", x => x.ItemSupplierId);
                    table.ForeignKey(
                        name: "FK_ItemSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemSuppliers_quotesItems_quotesItemId",
                        column: x => x.quotesItemId,
                        principalTable: "quotesItems",
                        principalColumn: "quotesItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSuppliers_quotesItemId",
                table: "ItemSuppliers",
                column: "quotesItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSuppliers_SupplierId",
                table: "ItemSuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_SignatureAccountantId",
                table: "quotes",
                column: "SignatureAccountantId");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_SignatureManagerId",
                table: "quotes",
                column: "SignatureManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_SignatureSuperVisorId",
                table: "quotes",
                column: "SignatureSuperVisorId");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_SignatureUserSecetaryId",
                table: "quotes",
                column: "SignatureUserSecetaryId");

            migrationBuilder.CreateIndex(
                name: "IX_quotesItems_quotesId",
                table: "quotesItems",
                column: "quotesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSuppliers");

            migrationBuilder.DropTable(
                name: "quotesItems");

            migrationBuilder.DropTable(
                name: "quotes");
        }
    }
}
