using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add RoleNumber to avoid duplicating ScientificProjects changes
            migrationBuilder.AddColumn<int>(
                name: "RoleNumber",
                schema: "Security",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleNumber",
                schema: "Security",
                table: "Roles");
        }
    }
}
