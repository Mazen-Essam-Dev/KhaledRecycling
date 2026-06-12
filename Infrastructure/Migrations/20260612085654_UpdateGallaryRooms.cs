using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGallaryRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilledKilo",
                table: "RoomGalleries");

            migrationBuilder.DropColumn(
                name: "MaxKilo",
                table: "RoomGalleries");

            migrationBuilder.DropColumn(
                name: "ReservedKilo",
                table: "RoomGalleries");

            migrationBuilder.AddColumn<int>(
                name: "FilledUnits",
                table: "RoomGalleries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxUnit",
                table: "RoomGalleries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedUnits",
                table: "RoomGalleries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilledUnits",
                table: "RoomGalleries");

            migrationBuilder.DropColumn(
                name: "MaxUnit",
                table: "RoomGalleries");

            migrationBuilder.DropColumn(
                name: "ReservedUnits",
                table: "RoomGalleries");

            migrationBuilder.AddColumn<double>(
                name: "FilledKilo",
                table: "RoomGalleries",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxKilo",
                table: "RoomGalleries",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReservedKilo",
                table: "RoomGalleries",
                type: "float",
                nullable: true);
        }
    }
}
