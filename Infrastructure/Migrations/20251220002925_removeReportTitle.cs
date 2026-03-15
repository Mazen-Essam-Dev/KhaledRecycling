using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeReportTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportTitle",
                table: "EstimatedBudgetForExternalParticipations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportTitle",
                table: "EstimatedBudgetForExternalParticipations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
