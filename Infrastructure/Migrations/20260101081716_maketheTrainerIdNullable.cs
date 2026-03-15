using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class maketheTrainerIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerId",
                table: "ScientificProjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerId",
                table: "ScientificProjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScientificProjects_Trainers_TrainerId",
                table: "ScientificProjects",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
