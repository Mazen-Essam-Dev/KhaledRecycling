using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalaryManagementAttachmentsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalaryManagementAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SalaryManagementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryManagementAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryManagementAttachments_SalaryManagements_SalaryManagementId",
                        column: x => x.SalaryManagementId,
                        principalTable: "SalaryManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryManagementAttachments_SalaryManagementId",
                table: "SalaryManagementAttachments",
                column: "SalaryManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryManagementAttachments");
        }
    }
}
