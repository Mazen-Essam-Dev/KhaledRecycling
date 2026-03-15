using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedSign2To1tableToSign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ExpensesSource_ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ItemTypes_ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropIndex(
                name: "IX_ExpensesAndReciptReportSigns_ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropIndex(
                name: "IX_ExpensesAndReciptReportSigns_ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropColumn(
                name: "ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropColumn(
                name: "ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.RenameColumn(
                name: "TypeTable",
                table: "ExpensesAndReciptReportSigns",
                newName: "ReportTypeId");

            migrationBuilder.CreateTable(
                name: "ReportType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ReportTypeId",
                table: "ExpensesAndReciptReportSigns",
                column: "ReportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportType_ReportTypeId",
                table: "ExpensesAndReciptReportSigns",
                column: "ReportTypeId",
                principalTable: "ReportType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ReportType_ReportTypeId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.DropTable(
                name: "ReportType");

            migrationBuilder.DropIndex(
                name: "IX_ExpensesAndReciptReportSigns_ReportTypeId",
                table: "ExpensesAndReciptReportSigns");

            migrationBuilder.RenameColumn(
                name: "ReportTypeId",
                table: "ExpensesAndReciptReportSigns",
                newName: "TypeTable");

            migrationBuilder.AddColumn<int>(
                name: "ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns",
                column: "ExpensesSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAndReciptReportSigns_ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns",
                column: "ItemTypeEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ExpensesSource_ExpensesSourceId",
                table: "ExpensesAndReciptReportSigns",
                column: "ExpensesSourceId",
                principalTable: "ExpensesSource",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensesAndReciptReportSigns_ItemTypes_ItemTypeEntityId",
                table: "ExpensesAndReciptReportSigns",
                column: "ItemTypeEntityId",
                principalTable: "ItemTypes",
                principalColumn: "Id");
        }
    }
}
