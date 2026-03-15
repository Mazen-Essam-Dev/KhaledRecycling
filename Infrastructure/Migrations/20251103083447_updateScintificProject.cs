using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateScintificProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityMonitorSignitureId",
                table: "ScientificProjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ScientificProjects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerSignitureId",
                table: "ScientificProjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjects_ActivityMonitorSignitureId",
                table: "ScientificProjects",
                column: "ActivityMonitorSignitureId");

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjects_ManagerSignitureId",
                table: "ScientificProjects",
                column: "ManagerSignitureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Signatures_ActivityMonitorSignitureId",
                table: "ScientificProjects",
                column: "ActivityMonitorSignitureId",
                principalTable: "Signatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Signatures_ManagerSignitureId",
                table: "ScientificProjects",
                column: "ManagerSignitureId",
                principalTable: "Signatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Signatures_ActivityMonitorSignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Signatures_ManagerSignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropIndex(
                name: "IX_ScientificProjects_ActivityMonitorSignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropIndex(
                name: "IX_ScientificProjects_ManagerSignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "ActivityMonitorSignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "ManagerSignitureId",
                table: "ScientificProjects");
        }
    }
}
