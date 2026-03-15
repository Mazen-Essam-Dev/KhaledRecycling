using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addNewTableIndividalsToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScientificProjectIndividuals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScientificProjectId = table.Column<int>(type: "int", nullable: false),
                    LangType = table.Column<int>(type: "int", nullable: false),
                    IndividualName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScientificProjectIndividuals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScientificProjectIndividuals_ScientificProjects_ScientificProjectId",
                        column: x => x.ScientificProjectId,
                        principalTable: "ScientificProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjectIndividuals_ScientificProjectId",
                table: "ScientificProjectIndividuals",
                column: "ScientificProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScientificProjectIndividuals");
        }
    }
}
