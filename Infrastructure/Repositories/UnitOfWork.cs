using Domain.Entities;
using Domain.Entities.Employees;

using Domain.Entities.MaterialOrder;

using Domain.Entities.SalaryManage;
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
      
        private IGenericRepository<Department> _Departments;
        private IGenericRepository<Employee> _Employees;
        private IGenericRepository<MemberEntity> _Members;
        private IGenericRepository<MemberType> _MemberTypes;
        private IGenericRepository<Nationality> _Nationalities;
        private IGenericRepository<RequestLog> _RequestLogs;
        private IGenericRepository<City> _Cities;
       
        private IGenericRepository<Signature> _Signatures;
      
        private IGenericRepository<SalaryManagement> _SalaryManagements;
        private IGenericRepository<SalaryManagementAttachment> _SalaryManagementAttachments;
     
        private IGenericRepository<EmployeeAttachment> _EmployeeAttachments;

        private IGenericRepository<Notification> _Notifications;
        private IGenericRepository<UserNotification> _UserNotifications;
   
        private IGenericRepository<SalaryReportSign> _SalaryReportSigns;
        private IGenericRepository<MaterialOrder> _MaterialOrders;
        private IGenericRepository<MaterialOrderItem> _MaterialOrderItems;
       

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

          
        }

        public ApplicationDbContext Context => _context;
      
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
        public IGenericRepository<Nationality> Nationalities => _Nationalities ??= new GenericRepository<Nationality>(_context);
        public IGenericRepository<RequestLog> RequestLogs => _RequestLogs ??= new GenericRepository<RequestLog>(_context);
        public IGenericRepository<City> Cities => _Cities ??= new GenericRepository<City>(_context);
     
        public IGenericRepository<Signature> Signatures => _Signatures ??= new GenericRepository<Signature>(_context);
              public IGenericRepository<SalaryManagement> SalaryManagements => _SalaryManagements ??= new GenericRepository<SalaryManagement>(_context);
        public IGenericRepository<SalaryManagementAttachment> SalaryManagementAttachments => _SalaryManagementAttachments ??= new GenericRepository<SalaryManagementAttachment>(_context);
              public IGenericRepository<EmployeeAttachment> EmployeeAttachments => _EmployeeAttachments ??= new GenericRepository<EmployeeAttachment>(_context);
        public IGenericRepository<Notification> Notifications => _Notifications ??= new GenericRepository<Notification>(_context);
        public IGenericRepository<UserNotification> UserNotifications => _UserNotifications ??= new GenericRepository<UserNotification>(_context);
            public IGenericRepository<SalaryReportSign> SalaryReportSigns => _SalaryReportSigns ??= new GenericRepository<SalaryReportSign>(_context);
        public IGenericRepository<MaterialOrder> MaterialOrders => _MaterialOrders ??= new GenericRepository<MaterialOrder>(_context);
        public IGenericRepository<MaterialOrderItem> MaterialOrderItems => _MaterialOrderItems ??= new GenericRepository<MaterialOrderItem>(_context);
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
