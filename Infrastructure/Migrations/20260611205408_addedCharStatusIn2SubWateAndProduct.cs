using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedCharStatusIn2SubWateAndProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusChar",
                table: "SubWastes",
                type: "nvarchar(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusChar",
                table: "SubProducts",
                type: "nvarchar(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusChar",
                table: "SubWastes");

            migrationBuilder.DropColumn(
                name: "StatusChar",
                table: "SubProducts");
        }
    }
}
