using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addExchangeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeProofDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExchangeProofId = table.Column<int>(type: "int", nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeProofDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeProofDetails_ExchangeProofs_ExchangeProofId",
                        column: x => x.ExchangeProofId,
                        principalTable: "ExchangeProofs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeProofDetails_ExchangeProofId",
                table: "ExchangeProofDetails",
                column: "ExchangeProofId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeProofDetails");
        }
    }
}
