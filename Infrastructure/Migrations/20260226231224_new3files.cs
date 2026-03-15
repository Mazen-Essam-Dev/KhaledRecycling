using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class new3files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePreliminaryPath1",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePreliminaryPath2",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePreliminaryPath3",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePreliminaryPath1",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "FilePreliminaryPath2",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "FilePreliminaryPath3",
                table: "ScientificProjects");
        }
    }
}
