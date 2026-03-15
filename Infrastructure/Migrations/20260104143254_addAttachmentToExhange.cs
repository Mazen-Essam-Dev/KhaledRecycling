using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAttachmentToExhange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashExchangeBondAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CashExchangeBondId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashExchangeBondAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashExchangeBondAttachments_CashExchangeBonds_CashExchangeBondId",
                        column: x => x.CashExchangeBondId,
                        principalTable: "CashExchangeBonds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashExchangeBondAttachments_CashExchangeBondId",
                table: "CashExchangeBondAttachments",
                column: "CashExchangeBondId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashExchangeBondAttachments");
        }
    }
}
