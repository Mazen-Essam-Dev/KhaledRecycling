using Domain.Entities;

using Domain.Entities.Employees;

using Domain.Entities.MaterialOrder;

using Domain.Entities.SalaryManage;
using Infrastructure.Identity;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.InterfacesDB
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<Department> Departments { get; }
        IGenericRepository<Employee> Employees { get; }
        IGenericRepository<MemberEntity> Members { get; }
        IGenericRepository<MemberType> MemberTypes { get; }
        IGenericRepository<Nationality> Nationalities { get; }
        IGenericRepository<RequestLog> RequestLogs { get; }
        IGenericRepository<City> Cities { get; }
    
        IGenericRepository<Signature> Signatures { get; }
     
        IGenericRepository<SalaryManagement> SalaryManagements { get; }
        IGenericRepository<SalaryManagementAttachment> SalaryManagementAttachments { get; }
       
        IGenericRepository<EmployeeAttachment> EmployeeAttachments { get; }
        IGenericRepository<Notification> Notifications { get; }

        IGenericRepository<UserNotification> UserNotifications { get; }
    
        IGenericRepository<SalaryReportSign> SalaryReportSigns { get; }
        IGenericRepository<MaterialOrder> MaterialOrders { get; }
        IGenericRepository<MaterialOrderItem> MaterialOrderItems { get; }
     

        // 👇 Add this property to expose DbContext
        ApplicationDbContext Context { get; }
        Task<int> CompleteAsync(); // SaveChanges
    }
}
