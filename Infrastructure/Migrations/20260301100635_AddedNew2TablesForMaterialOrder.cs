using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNew2TablesForMaterialOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialOrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    OrderTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SignatureUserId = table.Column<int>(type: "int", nullable: true),
                    SignatureManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialOrders_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialOrders_Signatures_SignatureManagerId",
                        column: x => x.SignatureManagerId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialOrders_Signatures_SignatureUserId",
                        column: x => x.SignatureUserId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialOrderItems",
                columns: table => new
                {
                    MaterialOrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialOrderId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    SinglePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialOrderItems", x => x.MaterialOrderItemId);
                    table.ForeignKey(
                        name: "FK_MaterialOrderItems_MaterialOrders_MaterialOrderId",
                        column: x => x.MaterialOrderId,
                        principalTable: "MaterialOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrderItems_MaterialOrderId",
                table: "MaterialOrderItems",
                column: "MaterialOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrders_DepartmentId",
                table: "MaterialOrders",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrders_SignatureManagerId",
                table: "MaterialOrders",
                column: "SignatureManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrders_SignatureUserId",
                table: "MaterialOrders",
                column: "SignatureUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialOrderItems");

            migrationBuilder.DropTable(
                name: "MaterialOrders");
        }
    }
}
