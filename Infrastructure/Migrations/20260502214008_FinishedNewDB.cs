using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinishedNewDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoRooms = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoRooms = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainWastes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainWastes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Method = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LogTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortChar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NationalityId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassportExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIdExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NationalIdLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Salary = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Nationalities_NationalityId",
                        column: x => x.NationalityId,
                        principalTable: "Nationalities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Financials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItsId = table.Column<int>(type: "int", nullable: true),
                    TypeTransaction = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Financials_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKMainProductId = table.Column<int>(type: "int", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellPriceUnit = table.Column<double>(type: "float", nullable: true),
                    SellPriceKilo = table.Column<double>(type: "float", nullable: true),
                    SellPriceTon = table.Column<double>(type: "float", nullable: true),
                    BuyPriceUnit = table.Column<double>(type: "float", nullable: true),
                    BuyPriceKilo = table.Column<double>(type: "float", nullable: true),
                    BuyPriceTon = table.Column<double>(type: "float", nullable: true),
                    RatioCountFor1Kilo = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProducts_MainProducts_FKMainProductId",
                        column: x => x.FKMainProductId,
                        principalTable: "MainProducts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubProducts_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubWastes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKMainWasteId = table.Column<int>(type: "int", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellPriceUnit = table.Column<double>(type: "float", nullable: true),
                    SellPriceKilo = table.Column<double>(type: "float", nullable: true),
                    SellPriceTon = table.Column<double>(type: "float", nullable: true),
                    BuyPriceUnit = table.Column<double>(type: "float", nullable: true),
                    BuyPriceKilo = table.Column<double>(type: "float", nullable: true),
                    BuyPriceTon = table.Column<double>(type: "float", nullable: true),
                    RatioCountFor1Kilo = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubWastes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubWastes_MainWastes_FKMainWasteId",
                        column: x => x.FKMainWasteId,
                        principalTable: "MainWastes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubWastes_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAttachment_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Allowances = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WorkDays = table.Column<int>(type: "int", nullable: false),
                    AbsentDays = table.Column<int>(type: "int", nullable: true),
                    SickLeaveDays = table.Column<int>(type: "int", nullable: true),
                    AnnualLeaveDays = table.Column<int>(type: "int", nullable: true),
                    DeductionsAddition = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BonusesAndMissions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryManagements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FkSubProduct = table.Column<int>(type: "int", nullable: true),
                    MaxKilo = table.Column<double>(type: "float", nullable: true),
                    FkGallery = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomGalleries_Galleries_FkGallery",
                        column: x => x.FkGallery,
                        principalTable: "Galleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomGalleries_SubProducts_FkSubProduct",
                        column: x => x.FkSubProduct,
                        principalTable: "SubProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FKSubWaste = table.Column<int>(type: "int", nullable: true),
                    MaxKilo = table.Column<double>(type: "float", nullable: true),
                    FkInventory = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomInventories_Inventories_FkInventory",
                        column: x => x.FkInventory,
                        principalTable: "Inventories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomInventories_SubWastes_FKSubWaste",
                        column: x => x.FKSubWaste,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FKSignature = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractSubProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKSubProduct = table.Column<int>(type: "int", nullable: true),
                    FKContract = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    Kilo = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSubProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSubProducts_Contracts_FKContract",
                        column: x => x.FKContract,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractSubProducts_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractSubProducts_SubProducts_FKSubProduct",
                        column: x => x.FKSubProduct,
                        principalTable: "SubProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractSubWastes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKSubWaste = table.Column<int>(type: "int", nullable: true),
                    FKContract = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    Kilo = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSubWastes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSubWastes_Contracts_FKContract",
                        column: x => x.FKContract,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractSubWastes_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractSubWastes_SubWastes_FKSubWaste",
                        column: x => x.FKSubWaste,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderBuyFromClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FKSubWasteId = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    Kilo = table.Column<double>(type: "float", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountRatio = table.Column<double>(type: "float", nullable: true),
                    DiscountValue = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBuyFromClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderBuyFromClients_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderBuyFromClients_SubWastes_FKSubWasteId",
                        column: x => x.FKSubWasteId,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderBuyFromFactories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FKSubWasteId = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    Kilo = table.Column<double>(type: "float", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountRatio = table.Column<double>(type: "float", nullable: true),
                    DiscountValue = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBuyFromFactories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderBuyFromFactories_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderBuyFromFactories_SubWastes_FKSubWasteId",
                        column: x => x.FKSubWasteId,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderSellToClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FKSubWasteId = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountRatio = table.Column<double>(type: "float", nullable: true),
                    DiscountValue = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSellToClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSellToClients_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderSellToClients_SubWastes_FKSubWasteId",
                        column: x => x.FKSubWasteId,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderSellToFactories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FKUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FKSubWasteId = table.Column<int>(type: "int", nullable: true),
                    CountUnits = table.Column<int>(type: "int", nullable: true),
                    Kilo = table.Column<double>(type: "float", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountRatio = table.Column<double>(type: "float", nullable: true),
                    DiscountValue = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSellToFactories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSellToFactories_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderSellToFactories_SubWastes_FKSubWasteId",
                        column: x => x.FKSubWasteId,
                        principalTable: "SubWastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FKUserType = table.Column<int>(type: "int", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommercialRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxRecordNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SignatureId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Signatures_SignatureId",
                        column: x => x.SignatureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_FKUserType",
                        column: x => x.FKUserType,
                        principalTable: "UserTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Security",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_UserNotifications_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Security",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Security",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ApplicationUserId",
                table: "Contracts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StatusId",
                table: "Contracts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubProducts_FKContract",
                table: "ContractSubProducts",
                column: "FKContract");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubProducts_FKSubProduct",
                table: "ContractSubProducts",
                column: "FKSubProduct");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubProducts_StatusId",
                table: "ContractSubProducts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubWastes_FKContract",
                table: "ContractSubWastes",
                column: "FKContract");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubWastes_FKSubWaste",
                table: "ContractSubWastes",
                column: "FKSubWaste");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubWastes_StatusId",
                table: "ContractSubWastes",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttachment_EmployeeId",
                table: "EmployeeAttachment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NationalityId",
                table: "Employees",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Financials_StatusId",
                table: "Financials",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromClients_FKSubWasteId",
                table: "OrderBuyFromClients",
                column: "FKSubWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromClients_FKUserId",
                table: "OrderBuyFromClients",
                column: "FKUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromClients_StatusId",
                table: "OrderBuyFromClients",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromFactories_FKSubWasteId",
                table: "OrderBuyFromFactories",
                column: "FKSubWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromFactories_FKUserId",
                table: "OrderBuyFromFactories",
                column: "FKUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyFromFactories_StatusId",
                table: "OrderBuyFromFactories",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToClients_FKSubWasteId",
                table: "OrderSellToClients",
                column: "FKSubWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToClients_FKUserId",
                table: "OrderSellToClients",
                column: "FKUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToClients_StatusId",
                table: "OrderSellToClients",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToFactories_FKSubWasteId",
                table: "OrderSellToFactories",
                column: "FKSubWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToFactories_FKUserId",
                table: "OrderSellToFactories",
                column: "FKUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSellToFactories_StatusId",
                table: "OrderSellToFactories",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Security",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Security",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomGalleries_FkGallery",
                table: "RoomGalleries",
                column: "FkGallery");

            migrationBuilder.CreateIndex(
                name: "IX_RoomGalleries_FkSubProduct",
                table: "RoomGalleries",
                column: "FkSubProduct");

            migrationBuilder.CreateIndex(
                name: "IX_RoomInventories_FkInventory",
                table: "RoomInventories",
                column: "FkInventory");

            migrationBuilder.CreateIndex(
                name: "IX_RoomInventories_FKSubWaste",
                table: "RoomInventories",
                column: "FKSubWaste");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryManagementAttachments_SalaryManagementId",
                table: "SalaryManagementAttachments",
                column: "SalaryManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryManagements_EmployeeId_Month_Year",
                table: "SalaryManagements",
                columns: new[] { "EmployeeId", "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_UserId",
                table: "Signatures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProducts_FKMainProductId",
                table: "SubProducts",
                column: "FKMainProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProducts_StatusId",
                table: "SubProducts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SubWastes_FKMainWasteId",
                table: "SubWastes",
                column: "FKMainWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_SubWastes_StatusId",
                table: "SubWastes",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Security",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Security",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_NotificationId",
                table: "UserNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Security",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Security",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FKUserType",
                schema: "Security",
                table: "Users",
                column: "FKUserType");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SignatureId",
                schema: "Security",
                table: "Users",
                column: "SignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                schema: "Security",
                table: "Users",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Security",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Users_ApplicationUserId",
                table: "Contracts",
                column: "ApplicationUserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromClients_Users_FKUserId",
                table: "OrderBuyFromClients",
                column: "FKUserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBuyFromFactories_Users_FKUserId",
                table: "OrderBuyFromFactories",
                column: "FKUserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToClients_Users_FKUserId",
                table: "OrderSellToClients",
                column: "FKUserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSellToFactories_Users_FKUserId",
                table: "OrderSellToFactories",
                column: "FKUserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Signatures_Users_UserId",
                table: "Signatures",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Statuses_StatusId",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Signatures_Users_UserId",
                table: "Signatures");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ContractSubProducts");

            migrationBuilder.DropTable(
                name: "ContractSubWastes");

            migrationBuilder.DropTable(
                name: "EmployeeAttachment");

            migrationBuilder.DropTable(
                name: "Financials");

            migrationBuilder.DropTable(
                name: "OrderBuyFromClients");

            migrationBuilder.DropTable(
                name: "OrderBuyFromFactories");

            migrationBuilder.DropTable(
                name: "OrderSellToClients");

            migrationBuilder.DropTable(
                name: "OrderSellToFactories");

            migrationBuilder.DropTable(
                name: "RequestLogs");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "RoomGalleries");

            migrationBuilder.DropTable(
                name: "RoomInventories");

            migrationBuilder.DropTable(
                name: "SalaryManagementAttachments");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "SubProducts");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "SubWastes");

            migrationBuilder.DropTable(
                name: "SalaryManagements");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "MainProducts");

            migrationBuilder.DropTable(
                name: "MainWastes");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
