using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryToDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentCategoryId",
                table: "ArchivingDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivingDocuments_DocumentCategoryId",
                table: "ArchivingDocuments",
                column: "DocumentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivingDocuments_DocumentCategories_DocumentCategoryId",
                table: "ArchivingDocuments",
                column: "DocumentCategoryId",
                principalTable: "DocumentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchivingDocuments_DocumentCategories_DocumentCategoryId",
                table: "ArchivingDocuments");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.DropIndex(
                name: "IX_ArchivingDocuments_DocumentCategoryId",
                table: "ArchivingDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentCategoryId",
                table: "ArchivingDocuments");
        }
    }
}
