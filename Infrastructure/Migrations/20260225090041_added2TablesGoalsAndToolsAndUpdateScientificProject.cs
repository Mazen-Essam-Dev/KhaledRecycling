using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added2TablesGoalsAndToolsAndUpdateScientificProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationAr",
                table: "ScientificProjects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationEn",
                table: "ScientificProjects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ClosedDate",
                table: "ScientificProjects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Innovation_Individual_TeamAr",
                table: "ScientificProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Innovation_Individual_TeamEn",
                table: "ScientificProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionAr",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionEn",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationAr",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationEn",
                table: "ScientificProjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Sponsorship",
                table: "ScientificProjects",
                type: "float",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Student_EmployeeAr",
                table: "ScientificProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Student_EmployeeEn",
                table: "ScientificProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScientificProjectGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScientificProjectId = table.Column<int>(type: "int", nullable: false),
                    LangType = table.Column<int>(type: "int", nullable: false),
                    GoalDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScientificProjectGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScientificProjectGoals_ScientificProjects_ScientificProjectId",
                        column: x => x.ScientificProjectId,
                        principalTable: "ScientificProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScientificProjectTools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScientificProjectId = table.Column<int>(type: "int", nullable: false),
                    LangType = table.Column<int>(type: "int", nullable: false),
                    ToolDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScientificProjectTools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScientificProjectTools_ScientificProjects_ScientificProjectId",
                        column: x => x.ScientificProjectId,
                        principalTable: "ScientificProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjectGoals_ScientificProjectId",
                table: "ScientificProjectGoals",
                column: "ScientificProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjectTools_ScientificProjectId",
                table: "ScientificProjectTools",
                column: "ScientificProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScientificProjectGoals");

            migrationBuilder.DropTable(
                name: "ScientificProjectTools");

            migrationBuilder.DropColumn(
                name: "ApplicationAr",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "ApplicationEn",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Innovation_Individual_TeamAr",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Innovation_Individual_TeamEn",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "InstitutionAr",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "InstitutionEn",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "OrganizationAr",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "OrganizationEn",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Sponsorship",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Student_EmployeeAr",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Student_EmployeeEn",
                table: "ScientificProjects");
        }
    }
}
