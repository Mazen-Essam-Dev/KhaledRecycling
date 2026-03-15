using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnDateFromAndToInEstemated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ParticipatingDate",
                table: "EstimatedBudgetForExternalParticipations",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ParticipatingDateTo",
                table: "EstimatedBudgetForExternalParticipations",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipatingDateTo",
                table: "EstimatedBudgetForExternalParticipations");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ParticipatingDate",
                table: "EstimatedBudgetForExternalParticipations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
