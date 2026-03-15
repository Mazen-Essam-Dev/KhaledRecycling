using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMissingRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReceivingReceipts_itemType",
                table: "ReceivingReceipts",
                column: "itemType");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReceipts_ItemTypes_itemType",
                table: "ReceivingReceipts",
                column: "itemType",
                principalTable: "ItemTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReceipts_ItemTypes_itemType",
                table: "ReceivingReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingReceipts_itemType",
                table: "ReceivingReceipts");
        }
    }
}
