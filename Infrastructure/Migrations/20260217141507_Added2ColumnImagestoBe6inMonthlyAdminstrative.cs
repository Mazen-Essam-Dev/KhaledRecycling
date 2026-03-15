using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added2ColumnImagestoBe6inMonthlyAdminstrative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image5Path",
                table: "MonthlyAdministrativeReports",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image6Path",
                table: "MonthlyAdministrativeReports",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image5Path",
                table: "MonthlyAdministrativeReports");

            migrationBuilder.DropColumn(
                name: "Image6Path",
                table: "MonthlyAdministrativeReports");
        }
    }
}
