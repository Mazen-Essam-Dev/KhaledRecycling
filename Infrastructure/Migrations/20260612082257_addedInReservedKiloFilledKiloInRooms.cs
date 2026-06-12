using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedInReservedKiloFilledKiloInRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FilledKilo",
                table: "RoomInventories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReservedKilo",
                table: "RoomInventories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FilledKilo",
                table: "RoomGalleries",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReservedKilo",
                table: "RoomGalleries",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilledKilo",
                table: "RoomInventories");

            migrationBuilder.DropColumn(
                name: "ReservedKilo",
                table: "RoomInventories");

            migrationBuilder.DropColumn(
                name: "FilledKilo",
                table: "RoomGalleries");

            migrationBuilder.DropColumn(
                name: "ReservedKilo",
                table: "RoomGalleries");
        }
    }
}
