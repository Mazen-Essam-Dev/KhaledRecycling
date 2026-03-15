using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSupplierCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierCategoryId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "SupplierCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SupplierCategories",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "عام", "General" },
                    { 2, "خاص", "Special" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierCategoryId",
                table: "Suppliers",
                column: "SupplierCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_SupplierCategories_SupplierCategoryId",
                table: "Suppliers",
                column: "SupplierCategoryId",
                principalTable: "SupplierCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_SupplierCategories_SupplierCategoryId",
                table: "Suppliers");

            migrationBuilder.DropTable(
                name: "SupplierCategories");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_SupplierCategoryId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierCategoryId",
                table: "Suppliers");
        }
    }
}
