using Application.Interfaces.Admin;
using Application.Services.Admin;
using KhaledTeamRecycling.Authorization;
using KhaledTeamRecycling.Helpers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Domain.DTOs.Admin.SMSDTO;

namespace KhaledTeamRecycling.Extensions
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
           
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<PermissionScanner>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<INotificationService , NotificationService>();


     

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<Application.Interfaces.Member.IAccountService, Application.Services.Member.AccountService>();
          
          
        
         
			services.AddScoped<ISalaryManagementService, SalaryManagementService>();
      
            services.AddScoped<IMaterialOrderService, MaterialOrderService>();

        }

    }

}
