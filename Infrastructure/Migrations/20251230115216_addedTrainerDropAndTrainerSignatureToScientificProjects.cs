using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedTrainerDropAndTrainerSignatureToScientificProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Trainer1SignitureId",
                table: "ScientificProjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainerId",
                table: "ScientificProjects",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjects_Trainer1SignitureId",
                table: "ScientificProjects",
                column: "Trainer1SignitureId");

            migrationBuilder.CreateIndex(
                name: "IX_ScientificProjects_TrainerId",
                table: "ScientificProjects",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Signatures_Trainer1SignitureId",
                table: "ScientificProjects",
                column: "Trainer1SignitureId",
                principalTable: "Signatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Signatures_Trainer1SignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects");

            migrationBuilder.DropIndex(
                name: "IX_ScientificProjects_Trainer1SignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropIndex(
                name: "IX_ScientificProjects_TrainerId",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "Trainer1SignitureId",
                table: "ScientificProjects");

            migrationBuilder.DropColumn(
                name: "TrainerId",
                table: "ScientificProjects");
        }
    }
}
