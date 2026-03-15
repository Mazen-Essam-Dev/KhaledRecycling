using Domain.Entities;
using Domain.Entities.BudgetItem;
using Domain.Entities.CashDisbursementVoucher;
using Domain.Entities.CashExchangeBond;
using Domain.Entities.Employees;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using Domain.Entities.ExpenseAndReceipt;
using Domain.Entities.MaterialOrder;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Entities.ParticipationsInEventReport;
using Domain.Entities.PurchaseOrder;
using Domain.Entities.QuartersReport;
using Domain.Entities.quote;
using Domain.Entities.SalaryManage;
using Domain.Entities.SMS;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext()
    {

    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ScientificProjects> ScientificProjects { get; set; }
    public DbSet<ScientificProjectGoals> ScientificProjectGoals { get; set; }
    public DbSet<ScientificProjectTools> ScientificProjectTools { get; set; }
    public DbSet<ScientificProjectIndividuals> ScientificProjectIndividuals { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<MemberType> MemberTypes { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Engineer> Engineers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarServiceEntity> CarServices { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Signature> Signatures { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierCategory> SupplierCategories { get; set; }
    public DbSet<AnnualSchedule> AnnualSchedules { get; set; }
    public DbSet<AnnualScheduleCategory> AnnualScheduleCategories { get; set; }
    public DbSet<MaintenanceClub> MaintenanceClubs { get; set; }
    public DbSet<MaintenanceClubAttachment> MaintenanceClubAttachments { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<ExternalWorkMission> ExternalWorkMissions { get; set; }
    public DbSet<MonthlyAdministrativeReport> MonthlyAdministrativeReports { get; set; }
    public DbSet<MonthlyAdministrativeReportDetail> MonthlyAdministrativeReportDetails { get; set; }
    public DbSet<EstimatedBudgetForExternalParticipation> EstimatedBudgetForExternalParticipations { get; set; }
    public DbSet<EstimatedBudgetForExternalParticipationDetail> EstimatedBudgetForExternalParticipationDetails { get; set; }
    public DbSet<ParticipationsInEventReport> ParticipationsInEventReports { get; set; }
    public DbSet<ParticipationsInEventReportDetail> ParticipationsInEventReportDetails { get; set; }
    public DbSet<SMS> SMS { get; set; }
    public DbSet<SMSReceiver> SMSReceivers { get; set; }
    public DbSet<BudgetItem> BudgetItems { get; set; }
    public DbSet<ExpenseAndReceiptAndOther> ExpenseAndReceiptAndOther { get; set; }
    public DbSet<ItemTypeEntity> ItemTypes { get; set; }
    public DbSet<ExpensesGate> ExpensesGate { get; set; }
    public DbSet<ExpensesSource> ExpensesSource { get; set; }
    public DbSet<ExpensesReport> ExpensesReports { get; set; }
    public DbSet<ExpenseAndReceiptReport> ExpenseAndReceiptReports { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<PurchaseOrderAttachment> PurchaseOrderAttachments { get; set; }
    public DbSet<SalaryManagement> SalaryManagements { get; set; }
    public DbSet<SalaryManagementAttachment> SalaryManagementAttachments { get; set; }
    
    public DbSet<CashDisbursementVoucher> CashDisbursementVouchers { get; set; }
    public DbSet<CashDisbursementVoucherDetail> CashDisbursementVoucherDetails { get; set; }
    public DbSet<CashDisbursementVoucherAttachment> CashDisbursementVoucherAttachments { get; set; }
    public DbSet<ExchangeProof> ExchangeProofs { get; set; }
    public DbSet<ExchangeProofDetail> ExchangeProofDetails { get; set; }
    public DbSet<ArchivingDocument> ArchivingDocuments { get; set; }
    public DbSet<DocumentCategory> DocumentCategories { get; set; }
    public DbSet<CashExchangeBond> CashExchangeBonds { get; set; }
    public DbSet<CashExchangeBondDetail> CashExchangeBondDetails { get; set; }
    public DbSet<CashExchangeBondAttachment> CashExchangeBondAttachments { get; set; }
    public DbSet<EmployeeAttachment> EmployeeAttachments { get; set; }
    public DbSet<CarAttachment> CarAttachments { get; set; }
    public DbSet<CarServiceAttachment> CarServiceAttachments { get; set; }
    public DbSet<OTP> OTPs { get; set; }
    public DbSet<QuartersReport> QuartersReports { get; set; }
    public DbSet<AcknowledgmentReceipt> AcknowledgmentReceipts { get; set; }


    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<ReceivingReceipt> ReceivingReceipts { get; set; }
    public DbSet<ExpensesAndReciptReportSign> ExpensesAndReciptReportSigns { get; set; }
    public DbSet<ReportType> ReportTypes { get; set; }
    public DbSet<ReportSalaryType> ReportSalaryTypes { get; set; }
    public DbSet<SalaryReportSign> SalaryReportSigns { get; set; }
    public DbSet<MaterialOrder> MaterialOrders { get; set; }
    public DbSet<MaterialOrderItem> MaterialOrderItems { get; set; }
    public DbSet<quote> quotes { get; set; }
    public DbSet<quotesItem> quotesItems { get; set; }
    public DbSet<ItemSupplier> ItemSuppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity Entities Added For Use Without DbSet only use inhirit from {ApplicationDbContext : IdentityDbContext<ApplicationUser>}
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Security");
        modelBuilder.Entity<ApplicationRole>().ToTable("Roles", "Security");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Security");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");

        // ✅ Uniqe Constrains (IdNumber)
        modelBuilder.Entity<MemberEntity>()
            .HasIndex(s => new { s.IdNumber })
            .IsUnique();

        modelBuilder.Entity<Trainer>()
       .HasOne<ApplicationUser>() // no nav property
       .WithMany()
       .HasForeignKey(t => t.UserId)
       .OnDelete(DeleteBehavior.Restrict); // or Cascade, if desired

        modelBuilder.Entity<Signature>()
       .HasOne<ApplicationUser>() // no nav property
       .WithMany()
       .HasForeignKey(t => t.UserId)
       .OnDelete(DeleteBehavior.Restrict); // or Cascade, if desired

        modelBuilder.Entity<PurchaseOrderItem>()
        .HasKey(p => new
        {
            p.PurchaseOrderItemId,
            p.PurchaseOrderId
        });

        // ✅ Uniqe Constrains (EmployeeId + Month + Year)
        modelBuilder.Entity<SalaryManagement>()
            .HasIndex(s => new { s.EmployeeId, s.Month, s.Year })
            .IsUnique();

        // ✅ Uniqe Constrains (Quarter + Type + Year)
        modelBuilder.Entity<QuartersReport>()
            .HasIndex(s => new { s.Quarter, s.Type, s.Year })
            .IsUnique();

        modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.RoleNumber)
                .HasDefaultValue(1);

        // ✅ Uniqe Constrains (IdNumber)
        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex(s => new { s.PurchaseOrderCode })
            .IsUnique();

        modelBuilder.Entity<UserNotification>()
        .HasKey(un => new { un.UserId, un.NotificationId });

        modelBuilder.Entity<UserNotification>()
            .HasOne<ApplicationUser>()           // 👈 No navigation in model, but tell EF the type
            .WithMany()                        // No navigation back to User
            .HasForeignKey(un => un.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserNotification>()
            .HasOne(un => un.Notification)
            .WithMany(n => n.UserNotifications)
            .HasForeignKey(un => un.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Supplier Category: seed two categories and default existing suppliers to category 1
        modelBuilder.Entity<SupplierCategory>()
            .HasData(
                new SupplierCategory { Id = 1, NameAr = "عام", NameEn = "General" },
                new SupplierCategory { Id = 2, NameAr = "خاص", NameEn = "Special" }
            );

        modelBuilder.Entity<Supplier>()
            .Property(s => s.SupplierCategoryId)
            .HasDefaultValue(1);

        modelBuilder.Entity<Supplier>()
            .HasOne(s => s.SupplierCategory)
            .WithMany(c => c.Suppliers)
            .HasForeignKey(s => s.SupplierCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ExchangeProof -> ExchangeProofDetail (one-to-many)
        modelBuilder.Entity<ExchangeProof>()
            .HasMany(e => e.Details)
            .WithOne(d => d.ExchangeProof)
            .HasForeignKey(d => d.ExchangeProofId)
            .OnDelete(DeleteBehavior.Cascade);

        // ReceivingReceipt ExpenseId
        modelBuilder.Entity<ReceivingReceipt>()
            .HasOne<ExpenseAndReceiptAndOther>()
            .WithMany()
            .HasForeignKey(un => un.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReceivingReceipt>()
            .HasOne(r => r.ExpenseAndReceiptAndOther)
            .WithMany()
            .HasForeignKey(r => r.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ItemType lookup: seed default values and link existing ItemType column as FK
        modelBuilder.Entity<ItemTypeEntity>()
            .HasData(
                new ItemTypeEntity { Id = 1, NameAr = "مصروفات", NameEn = "Expenses" },
                new ItemTypeEntity { Id = 2, NameAr = "مقبوضات", NameEn = "Receipts" }
            );

        // ReportTypes lookup: seed default values and link existing ItemType column as FK
        modelBuilder.Entity<ReportType>()
            .HasData(
                new ReportType { Id = 1, NameAr = " مصروفات نثرية", NameEn = "Expenses" },
                new ReportType { Id = 2, NameAr = "مقبوضات ومدفوعات", NameEn = "Expenses And Receipts" }
            );

        // ReportTypes lookup: seed default values and link existing ItemType column as FK
        modelBuilder.Entity<ReportSalaryType>()
            .HasData(
                new ReportSalaryType { Id = 1, NameAr = "تقرير الرواتب والاجور", NameEn = "Salary Report" },
                new ReportSalaryType { Id = 2, NameAr = "تقرير الخصومات والعلاوات", NameEn = "Discounts and Bonuses Report" },
                new ReportSalaryType { Id = 3, NameAr = "تقرير الغيابات", NameEn = "Absences Report" }
            );


        modelBuilder.Entity<ExpenseAndReceiptAndOther>()
            .Property(e => e.ItemType)
            .HasConversion<int?>();

        modelBuilder.Entity<ExpenseAndReceiptAndOther>()
            .HasOne(e => e.ItemTypeEntity)
            .WithMany(t => t.ExpenseAndReceiptAndOthers)
            .HasForeignKey(e => e.ItemType)
            .HasPrincipalKey(t => t.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // ArchivingDocument -> DocumentCategory (nullable FK)
        modelBuilder.Entity<ArchivingDocument>()
            .HasOne(ad => ad.DocumentCategory)
            .WithMany(dc => dc.ArchivingDocuments)
            .HasForeignKey(ad => ad.DocumentCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // AnnualScheduleCategory: seed three categories
        modelBuilder.Entity<AnnualScheduleCategory>()
            .HasData(
                new AnnualScheduleCategory { Id = 1, NameAr = "تخطيط", NameEn = "Planning" },
                new AnnualScheduleCategory { Id = 2, NameAr = "تنفيذ", NameEn = "Execution" },
                new AnnualScheduleCategory { Id = 3, NameAr = "مراجعة", NameEn = "Review" }
            );

        // AnnualSchedule -> AnnualScheduleCategory (nullable FK)
        modelBuilder.Entity<AnnualSchedule>()
            .HasOne(a => a.AnnualScheduleCategory)
            .WithMany(c => c.AnnualSchedules)
            .HasForeignKey(a => a.AnnualScheduleCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

    }



}
