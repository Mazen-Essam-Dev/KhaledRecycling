using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTableMemberTypeAndForignKeyInMemberEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberTypeId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberTypeId",
                table: "Members",
                column: "MemberTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MemberTypes_MemberTypeId",
                table: "Members",
                column: "MemberTypeId",
                principalTable: "MemberTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MemberTypes_MemberTypeId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MemberTypes");

            migrationBuilder.DropIndex(
                name: "IX_Members_MemberTypeId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberTypeId",
                table: "Members");
        }
    }
}
