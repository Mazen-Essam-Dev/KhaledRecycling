using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAnuualCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnualScheduleCategoryId",
                table: "AnnualSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnnualScheduleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualScheduleCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AnnualScheduleCategories",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "تخطيط", "Planning" },
                    { 2, "تنفيذ", "Execution" },
                    { 3, "مراجعة", "Review" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualSchedules_AnnualScheduleCategoryId",
                table: "AnnualSchedules",
                column: "AnnualScheduleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualSchedules_AnnualScheduleCategories_AnnualScheduleCategoryId",
                table: "AnnualSchedules",
                column: "AnnualScheduleCategoryId",
                principalTable: "AnnualScheduleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualSchedules_AnnualScheduleCategories_AnnualScheduleCategoryId",
                table: "AnnualSchedules");

            migrationBuilder.DropTable(
                name: "AnnualScheduleCategories");

            migrationBuilder.DropIndex(
                name: "IX_AnnualSchedules_AnnualScheduleCategoryId",
                table: "AnnualSchedules");

            migrationBuilder.DropColumn(
                name: "AnnualScheduleCategoryId",
                table: "AnnualSchedules");
        }
    }
}
