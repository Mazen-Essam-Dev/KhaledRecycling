using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added2TablesForSalaryReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportSalaryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSalaryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryReportSigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportSalaryTypeId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    AcountantSignatureId = table.Column<int>(type: "int", nullable: true),
                    ManagerSignitureId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryReportSigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryReportSigns_ReportSalaryTypes_ReportSalaryTypeId",
                        column: x => x.ReportSalaryTypeId,
                        principalTable: "ReportSalaryTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalaryReportSigns_Signatures_AcountantSignatureId",
                        column: x => x.AcountantSignatureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalaryReportSigns_Signatures_ManagerSignitureId",
                        column: x => x.ManagerSignitureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryReportSigns_AcountantSignatureId",
                table: "SalaryReportSigns",
                column: "AcountantSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryReportSigns_ManagerSignitureId",
                table: "SalaryReportSigns",
                column: "ManagerSignitureId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryReportSigns_ReportSalaryTypeId",
                table: "SalaryReportSigns",
                column: "ReportSalaryTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryReportSigns");

            migrationBuilder.DropTable(
                name: "ReportSalaryTypes");
        }
    }
}
