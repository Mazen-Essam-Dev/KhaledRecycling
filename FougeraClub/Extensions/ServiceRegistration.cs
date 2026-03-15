using Application.Interfaces.Admin;
using Application.Interfaces.Admin.ExpenseAndReceipt;
using Application.Services.Admin;
using Application.Services.Admin.ExpenseAndReceipt;
using Domain.DTOs.Admin.SMSDTO;
using FougeraClub.Authorization;
using FougeraClub.Helpers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FougeraClub.Extensions
{
    public static class ServiceRegistration
    {
        public static void ConfigureWebLayer(this IServiceCollection services,WebApplicationBuilder builder)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
        }
        public static void ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IExcelReportService<>), typeof(ExcelReportService<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //inject your services here
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddScoped<IRolesService, RolesService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddScoped<IEngineerService, EngineerService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<ICarServiceManager, CarServiceManager>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<PermissionScanner>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<INotificationService , NotificationService>();


            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<Application.Interfaces.Member.IActivityService, Application.Services.Member.ActivityService>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<Application.Interfaces.Member.IAccountService, Application.Services.Member.AccountService>();
            services.AddScoped<Application.Interfaces.Member.IOCRService, Application.Services.Member.OCRService>();
            services.AddScoped<Application.Interfaces.Member.ICompareService, Application.Services.Member.CompareService>();


            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IAnnualScheduleService, AnnualScheduleService>();
            services.AddScoped<IMaintenanceClubService, MaintenanceClubService>();
            services.AddScoped<IExternalWorkMissionService, ExternalWorkMissionService>();
            services.AddScoped<IEstimatedBudgetForExternalParticipationService, EstimatedBudgetForExternalParticipationService>();
            services.AddScoped<Application.Interfaces.Member.ICourseService, Application.Services.Member.CourseService>();

            services.AddScoped<IMonthlyAdministrativeReportService, MonthlyAdministrativeReportService>();
            services.AddScoped<IParticipationsInEventReportService, ParticipationsInEventReportService>();
            services.AddScoped<IAdministrativeReportQuarterlyAnnualService, AdministrativeReportQuarterlyAnnualService>();
            services.AddScoped<IAdministrativeReportQuarterlyAnnualService, AdministrativeReportQuarterlyAnnualService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

            //services.AddScoped<ISMSService, SMSService>();
            services.AddHttpClient<ISMSService, SMSService>();

            services.AddScoped<ISMSForSendingOTPService, SMSForSendingOTPService>();

            services.AddScoped<IBudgetItemService, BudgetItemService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IReceiptService, ReceiptService>();
			services.AddScoped<ISalaryManagementService, SalaryManagementService>();
            services.AddScoped<IArchivingDocumentService, ArchivingDocumentService>();
            services.AddScoped<ICashExchangeBondService, CashExchangeBondService>();
            services.AddScoped<ICashDisbursementVoucherService, CashDisbursementVoucherService>();
            services.AddScoped<IScientificProjectsService, ScientificProjectsService>();
            services.AddScoped<IMaterialOrderService, MaterialOrderService>();
            services.AddScoped<IquoteService, quoteService>();

        }

    }

}
