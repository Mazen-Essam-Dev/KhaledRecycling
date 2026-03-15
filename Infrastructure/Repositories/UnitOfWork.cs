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
using Infrastructure.Repositories.InterfacesDB;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private IGenericRepository<ApplicationUser> _Users;
        private IGenericRepository<Activity> _Activities;
        private IGenericRepository<Course> _Courses;
        private IGenericRepository<Department> _Departments;
        private IGenericRepository<Employee> _Employees;
        private IGenericRepository<MemberEntity> _Members;
        private IGenericRepository<MemberType> _MemberTypes;
        private IGenericRepository<Subscription> _Subscriptions;
        private IGenericRepository<Nationality> _Nationalities;
        private IGenericRepository<Question> _Questions;
        private IGenericRepository<RequestLog> _RequestLogs;
        private IGenericRepository<City> _Cities;
        private IGenericRepository<Engineer> _Engineers;
        private IGenericRepository<ScientificProjects> _ScientificProjects;
        private IGenericRepository<ScientificProjectGoals> _ScientificProjectGoals;
        private IGenericRepository<ScientificProjectTools> _ScientificProjectTools;
        private IGenericRepository<ScientificProjectIndividuals> _ScientificProjectIndividuals;
        private IGenericRepository<Car> _Cars;
        private IGenericRepository<CarServiceEntity> _CarServices;
        private IGenericRepository<Trainer> _Trainers;
        private IGenericRepository<Signature> _Signatures;
        private IGenericRepository<Supplier> _Suppliers;
        private IGenericRepository<SupplierCategory> _SupplierCategorys;
        private IGenericRepository<AnnualSchedule> _AnnualSchedules;
        private IGenericRepository<AnnualScheduleCategory> _AnnualScheduleCategories;
        private IGenericRepository<MaintenanceClub> _MaintenanceClubs;
        private IGenericRepository<MaintenanceClubAttachment> _MaintenanceClubAttachments;
        private IGenericRepository<Mission> _Missions;
        private IGenericRepository<ExternalWorkMission> _ExternalWorkMissions;
        private IGenericRepository<MonthlyAdministrativeReport> _MonthlyAdministrativeReports;
        private IGenericRepository<MonthlyAdministrativeReportDetail> _MonthlyAdministrativeReportDetails;
        private IGenericRepository<EstimatedBudgetForExternalParticipation> _EstimatedBudgetForExternalParticipations;
        private IGenericRepository<EstimatedBudgetForExternalParticipationDetail> _EstimatedBudgetForExternalParticipationDetails;
        private IGenericRepository<ParticipationsInEventReport> _ParticipationsInEventReports;
        private IGenericRepository<ParticipationsInEventReportDetail> _ParticipationsInEventReportDetails;
        private IGenericRepository<SMS> _SMS;
        private IGenericRepository<SMSReceiver> _SMSReceivers;
        private IGenericRepository<BudgetItem> _BudgetItems;
        private IGenericRepository<ExpenseAndReceiptAndOther> _ExpenseAndReceiptAndOthers;
        private IGenericRepository<ExpensesGate> _ExpensesGates;
        private IGenericRepository<ExpensesSource> _ExpensesSources;
        private IGenericRepository<ExpensesReport> _ExpensesReports;
        private IGenericRepository<ExpenseAndReceiptReport> _ExpenseAndReceiptReports;
        private IGenericRepository<PurchaseOrder> _PurchaseOrders;
        private IGenericRepository<PurchaseOrderItem> _PurchaseOrderItems;
        private IGenericRepository<PurchaseOrderAttachment> _PurchaseOrderAttachments;
        private IGenericRepository<SalaryManagement> _SalaryManagements;
        private IGenericRepository<SalaryManagementAttachment> _SalaryManagementAttachments;
        private IGenericRepository<CashExchangeBond> _CashExchangeBonds;
        private IGenericRepository<CashExchangeBondDetail> _CashExchangeBondDetails;
        private IGenericRepository<CashExchangeBondAttachment> _CashExchangeBondAttachments;
        private IGenericRepository<ArchivingDocument> _ArchivingDocuments;
        private IGenericRepository<DocumentCategory> _DocumentCategories;
        private IGenericRepository<CashDisbursementVoucher> _CashDisbursementVouchers;
        private IGenericRepository<CashDisbursementVoucherDetail> _CashDisbursementVoucherDetails;
        private IGenericRepository<CashDisbursementVoucherAttachment> _CashDisbursementVoucherAttachments;
        private IGenericRepository<ExchangeProof> _ExchangeProofs;
        private IGenericRepository<EmployeeAttachment> _EmployeeAttachments;
        private IGenericRepository<CarAttachment> _CarAttachments;
        private IGenericRepository<CarServiceAttachment> _CarServiceAttachments;
        private IGenericRepository<OTP> _OTPs;
        private IGenericRepository<QuartersReport> _QuartersReports;
        private IGenericRepository<AcknowledgmentReceipt> _AcknowledgmentReceipts;
        private IGenericRepository<ReceivingReceipt> _ReceivingReceipts;
        private IGenericRepository<Notification> _Notifications;
        private IGenericRepository<UserNotification> _UserNotifications;
        private IGenericRepository<ReportType> _ReportTypes;
        private IGenericRepository<ExpensesAndReciptReportSign> _ExpensesAndReciptReportSigns;
        private IGenericRepository<ReportSalaryType> _ReportSalaryTypes;
        private IGenericRepository<SalaryReportSign> _SalaryReportSigns;
        private IGenericRepository<MaterialOrder> _MaterialOrders;
        private IGenericRepository<MaterialOrderItem> _MaterialOrderItems;
        private IGenericRepository<quote> _quotes;
        private IGenericRepository<quotesItem> _quotesItems;
        private IGenericRepository<ItemSupplier> _ItemSuppliers;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            _Suppliers = new GenericRepository<Supplier>(_context);
            _ExpenseAndReceiptAndOthers = new GenericRepository<ExpenseAndReceiptAndOther>(_context);
            _ExpensesSources = new GenericRepository<ExpensesSource>(_context);
            _ExpensesGates = new GenericRepository<ExpensesGate>(_context);
            _PurchaseOrders = new GenericRepository<PurchaseOrder>(_context);
        }

        public ApplicationDbContext Context => _context;
        public IGenericRepository<Course> Courses => _Courses ??= new GenericRepository<Course>(_context);
        public IGenericRepository<Activity> Activities
            => _Activities ??= new GenericRepository<Activity>(_context);
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repositoryInstance = new GenericRepository<T>(_context);
            _repositories[typeof(T)] = repositoryInstance;
            return repositoryInstance;
        }
        public IGenericRepository<ApplicationUser> Users => _Users ??= new GenericRepository<ApplicationUser>(_context);
        public IGenericRepository<Department> Departments => _Departments ??= new GenericRepository<Department>(_context);
        public IGenericRepository<Employee> Employees => _Employees ??= new GenericRepository<Employee>(_context);
        public IGenericRepository<MemberEntity> Members => _Members ??= new GenericRepository<MemberEntity>(_context);
        public IGenericRepository<MemberType> MemberTypes => _MemberTypes ??= new GenericRepository<MemberType>(_context);
        public IGenericRepository<Subscription> Subscriptions => _Subscriptions ??= new GenericRepository<Subscription>(_context);
        public IGenericRepository<Nationality> Nationalities => _Nationalities ??= new GenericRepository<Nationality>(_context);
        public IGenericRepository<Question> Questions => _Questions ??= new GenericRepository<Question>(_context);
        public IGenericRepository<RequestLog> RequestLogs => _RequestLogs ??= new GenericRepository<RequestLog>(_context);
        public IGenericRepository<City> Cities => _Cities ??= new GenericRepository<City>(_context);
        public IGenericRepository<Engineer> Engineers => _Engineers ??= new GenericRepository<Engineer>(_context);
        public IGenericRepository<ScientificProjects> ScientificProjects => _ScientificProjects ??= new GenericRepository<ScientificProjects>(_context);
        public IGenericRepository<ScientificProjectGoals> ScientificProjectGoals => _ScientificProjectGoals ??= new GenericRepository<ScientificProjectGoals>(_context);
        public IGenericRepository<ScientificProjectTools> ScientificProjectTools => _ScientificProjectTools ??= new GenericRepository<ScientificProjectTools>(_context);
        public IGenericRepository<ScientificProjectIndividuals> ScientificProjectIndividuals => _ScientificProjectIndividuals ??= new GenericRepository<ScientificProjectIndividuals>(_context);
        public IGenericRepository<Car> Cars => _Cars ??= new GenericRepository<Car>(_context);
        public IGenericRepository<CarServiceEntity> CarServices => _CarServices ??= new GenericRepository<CarServiceEntity>(_context);
        public IGenericRepository<Trainer> Trainers => _Trainers ??= new GenericRepository<Trainer>(_context);
        public IGenericRepository<Signature> Signatures => _Signatures ??= new GenericRepository<Signature>(_context);
        public IGenericRepository<Supplier> Suppliers => _Suppliers ??= new GenericRepository<Supplier>(_context);
        public IGenericRepository<SupplierCategory> SupplierCategorys => _SupplierCategorys ??= new GenericRepository<SupplierCategory>(_context);
        public IGenericRepository<AnnualSchedule> AnnualSchedules => _AnnualSchedules ??= new GenericRepository<AnnualSchedule>(_context);
        public IGenericRepository<AnnualScheduleCategory> AnnualScheduleCategories => _AnnualScheduleCategories ??= new GenericRepository<AnnualScheduleCategory>(_context);
        public IGenericRepository<MaintenanceClub> MaintenanceClubs => _MaintenanceClubs ??= new GenericRepository<MaintenanceClub>(_context);
        public IGenericRepository<MaintenanceClubAttachment> MaintenanceClubAttachments => _MaintenanceClubAttachments ??= new GenericRepository<MaintenanceClubAttachment>(_context);
        public IGenericRepository<ExternalWorkMission> ExternalWorkMissions => _ExternalWorkMissions ??= new GenericRepository<ExternalWorkMission>(_context);
        public IGenericRepository<Mission> Missions => _Missions ??= new GenericRepository<Mission>(_context);
        public IGenericRepository<MonthlyAdministrativeReport> MonthlyAdministrativeReports => _MonthlyAdministrativeReports ??= new GenericRepository<MonthlyAdministrativeReport>(_context);
        public IGenericRepository<MonthlyAdministrativeReportDetail> MonthlyAdministrativeReportDetails => _MonthlyAdministrativeReportDetails ??= new GenericRepository<MonthlyAdministrativeReportDetail>(_context);
        public IGenericRepository<EstimatedBudgetForExternalParticipation> EstimatedBudgetForExternalParticipations => _EstimatedBudgetForExternalParticipations ??= new GenericRepository<EstimatedBudgetForExternalParticipation>(_context);
        public IGenericRepository<EstimatedBudgetForExternalParticipationDetail> EstimatedBudgetForExternalParticipationDetails => _EstimatedBudgetForExternalParticipationDetails ??= new GenericRepository<EstimatedBudgetForExternalParticipationDetail>(_context);
        public IGenericRepository<ParticipationsInEventReport> ParticipationsInEventReports => _ParticipationsInEventReports ??= new GenericRepository<ParticipationsInEventReport>(_context);
        public IGenericRepository<ParticipationsInEventReportDetail> ParticipationsInEventReportDetails => _ParticipationsInEventReportDetails ??= new GenericRepository<ParticipationsInEventReportDetail>(_context);
        public IGenericRepository<SMS> SMS => _SMS ??= new GenericRepository<SMS>(_context);
        public IGenericRepository<SMSReceiver> SMSReceivers => _SMSReceivers ??= new GenericRepository<SMSReceiver>(_context);
        public IGenericRepository<BudgetItem> BudgetItems => _BudgetItems ??= new GenericRepository<BudgetItem>(_context);
        public IGenericRepository<ExpenseAndReceiptAndOther> ExpenseAndReceiptAndOther => _ExpenseAndReceiptAndOthers ??= new GenericRepository<ExpenseAndReceiptAndOther>(_context);
        public IGenericRepository<ExpensesSource> ExpensesSources => _ExpensesSources ??= new GenericRepository<ExpensesSource>(_context);
        public IGenericRepository<ExpensesGate> ExpensesGates => _ExpensesGates ??= new GenericRepository<ExpensesGate>(_context);
        public IGenericRepository<ExpensesReport> ExpensesReports => _ExpensesReports ??= new GenericRepository<ExpensesReport>(_context);
        public IGenericRepository<ExpenseAndReceiptReport> ExpenseAndReceiptReports => _ExpenseAndReceiptReports ??= new GenericRepository<ExpenseAndReceiptReport>(_context);
        public IGenericRepository<PurchaseOrder> PurchaseOrders => _PurchaseOrders ??= new GenericRepository<PurchaseOrder>(_context);
        public IGenericRepository<PurchaseOrderItem> PurchaseOrderItems => _PurchaseOrderItems ??= new GenericRepository<PurchaseOrderItem>(_context);
        public IGenericRepository<PurchaseOrderAttachment> PurchaseOrderAttachments => _PurchaseOrderAttachments ??= new GenericRepository<PurchaseOrderAttachment>(_context);
        public IGenericRepository<SalaryManagement> SalaryManagements => _SalaryManagements ??= new GenericRepository<SalaryManagement>(_context);
        public IGenericRepository<SalaryManagementAttachment> SalaryManagementAttachments => _SalaryManagementAttachments ??= new GenericRepository<SalaryManagementAttachment>(_context);
        public IGenericRepository<CashDisbursementVoucher> CashDisbursementVouchers => _CashDisbursementVouchers ??= new GenericRepository<CashDisbursementVoucher>(_context);
        public IGenericRepository<CashDisbursementVoucherDetail> CashDisbursementVoucherDetails => _CashDisbursementVoucherDetails ??= new GenericRepository<CashDisbursementVoucherDetail>(_context);
        public IGenericRepository<CashDisbursementVoucherAttachment> CashDisbursementVoucherAttachments => _CashDisbursementVoucherAttachments ??= new GenericRepository<CashDisbursementVoucherAttachment>(_context);
        public IGenericRepository<ExchangeProof> ExchangeProofs => _ExchangeProofs ??= new GenericRepository<ExchangeProof>(_context);
        public IGenericRepository<ArchivingDocument> ArchivingDocuments => _ArchivingDocuments ??= new GenericRepository<ArchivingDocument>(_context);
        public IGenericRepository<DocumentCategory> DocumentCategories => _DocumentCategories ??= new GenericRepository<DocumentCategory>(_context);
        public IGenericRepository<CashExchangeBond> CashExchangeBonds => _CashExchangeBonds ??= new GenericRepository<CashExchangeBond>(_context);
        public IGenericRepository<CashExchangeBondDetail> CashExchangeBondDetails => _CashExchangeBondDetails ??= new GenericRepository<CashExchangeBondDetail>(_context);
        public IGenericRepository<CashExchangeBondAttachment> CashExchangeBondAttachments => _CashExchangeBondAttachments ??= new GenericRepository<CashExchangeBondAttachment>(_context);
        public IGenericRepository<EmployeeAttachment> EmployeeAttachments => _EmployeeAttachments ??= new GenericRepository<EmployeeAttachment>(_context);
        public IGenericRepository<CarAttachment> CarAttachments => _CarAttachments ??= new GenericRepository<CarAttachment>(_context);
        public IGenericRepository<CarServiceAttachment> CarServiceAttachments => _CarServiceAttachments ??= new GenericRepository<CarServiceAttachment>(_context);
        public IGenericRepository<OTP> OTPs => _OTPs ??= new GenericRepository<OTP>(_context);
        public IGenericRepository<QuartersReport> QuartersReports => _QuartersReports ??= new GenericRepository<QuartersReport>(_context);
        public IGenericRepository<AcknowledgmentReceipt> AcknowledgmentReceipts => _AcknowledgmentReceipts ??= new GenericRepository<AcknowledgmentReceipt>(_context);
        public IGenericRepository<ReceivingReceipt> ReceivingReceipts => _ReceivingReceipts ??= new GenericRepository<ReceivingReceipt>(_context);

        public IGenericRepository<Notification> Notifications => _Notifications ??= new GenericRepository<Notification>(_context);
        public IGenericRepository<UserNotification> UserNotifications => _UserNotifications ??= new GenericRepository<UserNotification>(_context);
        public IGenericRepository<ExpensesAndReciptReportSign> ExpensesAndReciptReportSigns => _ExpensesAndReciptReportSigns ??= new GenericRepository<ExpensesAndReciptReportSign>(_context);
        public IGenericRepository<ReportType> ReportTypes => _ReportTypes ??= new GenericRepository<ReportType>(_context);
        public IGenericRepository<SalaryReportSign> SalaryReportSigns => _SalaryReportSigns ??= new GenericRepository<SalaryReportSign>(_context);
        public IGenericRepository<ReportSalaryType> ReportSalaryTypes => _ReportSalaryTypes ??= new GenericRepository<ReportSalaryType>(_context);
        public IGenericRepository<MaterialOrder> MaterialOrders => _MaterialOrders ??= new GenericRepository<MaterialOrder>(_context);
        public IGenericRepository<MaterialOrderItem> MaterialOrderItems => _MaterialOrderItems ??= new GenericRepository<MaterialOrderItem>(_context);
        public IGenericRepository<quote> quotes => _quotes ??= new GenericRepository<quote>(_context);
        public IGenericRepository<quotesItem> quotesItems => _quotesItems ??= new GenericRepository<quotesItem>(_context);
        public IGenericRepository<ItemSupplier> ItemSuppliers => _ItemSuppliers ??= new GenericRepository<ItemSupplier>(_context);
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
