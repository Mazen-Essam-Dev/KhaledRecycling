using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAttachement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashDisbursementVoucherAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CashDisbursementVoucherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDisbursementVoucherAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashDisbursementVoucherAttachments_CashDisbursementVouchers_CashDisbursementVoucherId",
                        column: x => x.CashDisbursementVoucherId,
                        principalTable: "CashDisbursementVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursementVoucherAttachments_CashDisbursementVoucherId",
                table: "CashDisbursementVoucherAttachments",
                column: "CashDisbursementVoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashDisbursementVoucherAttachments");
        }
    }
}
