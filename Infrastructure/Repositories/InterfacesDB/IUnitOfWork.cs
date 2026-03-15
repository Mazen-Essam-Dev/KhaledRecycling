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
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.InterfacesDB
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<Activity> Activities { get; }
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<Department> Departments { get; }
        IGenericRepository<Employee> Employees { get; }
        IGenericRepository<MemberEntity> Members { get; }
        IGenericRepository<MemberType> MemberTypes { get; }
        IGenericRepository<Subscription> Subscriptions { get; }
        IGenericRepository<Nationality> Nationalities { get; }
        IGenericRepository<Question> Questions { get; }
        IGenericRepository<RequestLog> RequestLogs { get; }
        IGenericRepository<City> Cities { get; }
        IGenericRepository<Engineer> Engineers { get; }
        IGenericRepository<ScientificProjects> ScientificProjects { get; }
        IGenericRepository<ScientificProjectGoals> ScientificProjectGoals { get; }
        IGenericRepository<ScientificProjectTools> ScientificProjectTools { get; }
        IGenericRepository<ScientificProjectIndividuals> ScientificProjectIndividuals { get; }
        IGenericRepository<Car> Cars { get; }
        IGenericRepository<CarServiceEntity> CarServices { get; }
        IGenericRepository<Trainer> Trainers { get; }
        IGenericRepository<Signature> Signatures { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<SupplierCategory> SupplierCategorys { get; }
        IGenericRepository<AnnualSchedule> AnnualSchedules { get; }
        IGenericRepository<AnnualScheduleCategory> AnnualScheduleCategories { get; }
        IGenericRepository<MaintenanceClub> MaintenanceClubs { get; }
        IGenericRepository<MaintenanceClubAttachment> MaintenanceClubAttachments { get; }
        IGenericRepository<ExternalWorkMission> ExternalWorkMissions { get; }
        IGenericRepository<Mission> Missions { get; }
        IGenericRepository<MonthlyAdministrativeReport> MonthlyAdministrativeReports { get; }
        IGenericRepository<MonthlyAdministrativeReportDetail> MonthlyAdministrativeReportDetails { get; }
        IGenericRepository<EstimatedBudgetForExternalParticipation> EstimatedBudgetForExternalParticipations { get; }
        IGenericRepository<EstimatedBudgetForExternalParticipationDetail> EstimatedBudgetForExternalParticipationDetails { get; }
        IGenericRepository<ParticipationsInEventReport> ParticipationsInEventReports { get; }
        IGenericRepository<ParticipationsInEventReportDetail> ParticipationsInEventReportDetails { get; }
        IGenericRepository<SMS> SMS { get; }
        IGenericRepository<SMSReceiver> SMSReceivers { get; }
        IGenericRepository<BudgetItem> BudgetItems { get; }
        IGenericRepository<ExpenseAndReceiptAndOther> ExpenseAndReceiptAndOther { get; }
        IGenericRepository<ExpensesGate> ExpensesGates { get; }
        IGenericRepository<ExpensesSource> ExpensesSources { get; }
        IGenericRepository<ExpensesReport> ExpensesReports { get; }
        IGenericRepository<ExpenseAndReceiptReport> ExpenseAndReceiptReports { get; }
        IGenericRepository<PurchaseOrder> PurchaseOrders { get; }
        IGenericRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
        IGenericRepository<PurchaseOrderAttachment> PurchaseOrderAttachments { get; }
        IGenericRepository<SalaryManagement> SalaryManagements { get; }
        IGenericRepository<SalaryManagementAttachment> SalaryManagementAttachments { get; }
        IGenericRepository<CashDisbursementVoucher> CashDisbursementVouchers { get; }
        IGenericRepository<CashDisbursementVoucherDetail> CashDisbursementVoucherDetails { get; }
        IGenericRepository<CashDisbursementVoucherAttachment> CashDisbursementVoucherAttachments { get; }
        IGenericRepository<ExchangeProof> ExchangeProofs { get; }
        IGenericRepository<ArchivingDocument> ArchivingDocuments { get; }
        IGenericRepository<DocumentCategory> DocumentCategories { get; }
        IGenericRepository<CashExchangeBond> CashExchangeBonds { get; }
        IGenericRepository<CashExchangeBondDetail> CashExchangeBondDetails { get; }
        IGenericRepository<CashExchangeBondAttachment> CashExchangeBondAttachments { get; }
        IGenericRepository<EmployeeAttachment> EmployeeAttachments { get; }
        IGenericRepository<CarAttachment> CarAttachments { get; }
        IGenericRepository<CarServiceAttachment> CarServiceAttachments { get; }
        IGenericRepository<OTP> OTPs { get; }
        IGenericRepository<QuartersReport> QuartersReports { get; }
        IGenericRepository<AcknowledgmentReceipt> AcknowledgmentReceipts { get; }
        IGenericRepository<ReceivingReceipt> ReceivingReceipts { get; }

        IGenericRepository<Notification> Notifications { get; }

        IGenericRepository<UserNotification> UserNotifications { get; }
        IGenericRepository<ReportType> ReportTypes { get; }
        IGenericRepository<ExpensesAndReciptReportSign> ExpensesAndReciptReportSigns { get; }
        IGenericRepository<SalaryReportSign> SalaryReportSigns { get; }
        IGenericRepository<ReportSalaryType> ReportSalaryTypes { get; }
        IGenericRepository<MaterialOrder> MaterialOrders { get; }
        IGenericRepository<MaterialOrderItem> MaterialOrderItems { get; }
        IGenericRepository<quote> quotes { get; }
        IGenericRepository<quotesItem> quotesItems { get; }
        IGenericRepository<ItemSupplier> ItemSuppliers { get; }

        // 👇 Add this property to expose DbContext
        ApplicationDbContext Context { get; }
        Task<int> CompleteAsync(); // SaveChanges
    }
}
