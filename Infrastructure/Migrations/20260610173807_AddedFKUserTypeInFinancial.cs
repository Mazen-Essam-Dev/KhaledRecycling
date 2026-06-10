using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFKUserTypeInFinancial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FKUserType",
                table: "Financials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Financials_FKUserType",
                table: "Financials",
                column: "FKUserType");

            migrationBuilder.AddForeignKey(
                name: "FK_Financials_UserTypes_FKUserType",
                table: "Financials",
                column: "FKUserType",
                principalTable: "UserTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Financials_UserTypes_FKUserType",
                table: "Financials");

            migrationBuilder.DropIndex(
                name: "IX_Financials_FKUserType",
                table: "Financials");

            migrationBuilder.DropColumn(
                name: "FKUserType",
                table: "Financials");
        }
    }
}
