using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAttachmentToClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceClubAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaintenanceClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceClubAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceClubAttachments_MaintenanceClubs_MaintenanceClubId",
                        column: x => x.MaintenanceClubId,
                        principalTable: "MaintenanceClubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceClubAttachments_MaintenanceClubId",
                table: "MaintenanceClubAttachments",
                column: "MaintenanceClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceClubAttachments");
        }
    }
}
