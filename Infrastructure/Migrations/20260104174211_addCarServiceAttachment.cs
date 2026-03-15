using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCarServiceAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarServiceAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CarServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarServiceAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarServiceAttachments_CarServices_CarServiceId",
                        column: x => x.CarServiceId,
                        principalTable: "CarServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarServiceAttachments_CarServiceId",
                table: "CarServiceAttachments",
                column: "CarServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarServiceAttachments");
        }
    }
}
