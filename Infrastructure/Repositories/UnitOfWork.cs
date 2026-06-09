using Domain.Entities;
using Domain.Entities.Contract;
using Domain.Entities.Employees;
using Domain.Entities.Gallary;
using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Domain.Entities.SalaryManage;
using Domain.Entities.Waste;
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
      
        private IGenericRepository<Employee> _Employees;
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

        private IGenericRepository<UserType> _UserTypes;

        private IGenericRepository<Contract> _Contracts;
        private IGenericRepository<ContractSubWaste> _ContractSubWastes;
        private IGenericRepository<ContractSubProduct> _ContractSubProducts;

        private IGenericRepository<MainWaste> _MainWastes;
        private IGenericRepository<SubWaste> _SubWastes;

        private IGenericRepository<MainProduct> _MainProducts;
        private IGenericRepository<SubProduct> _SubProducts;

        private IGenericRepository<OrderBuyFromClient> _OrderBuyFromClients;
        private IGenericRepository<OrderBuyFromClientAttachment> _OrderBuyFromClientAttachments;
        private IGenericRepository<OrderSellToFactory> _OrderSellToFactories;
        private IGenericRepository<OrderSellToFactoryAttachment> _OrderSellToFactoryAttachments;
        private IGenericRepository<OrderBuyFromFactory> _OrderBuyFromFactories;
        private IGenericRepository<OrderBuyFromFactoryAttachment> _OrderBuyFromFactoryAttachments;
        private IGenericRepository<OrderSellToClient> _OrderSellToClients;
        private IGenericRepository<OrderSellToClientAttachment> _OrderSellToClientAttachments;

        private IGenericRepository<Financial> _Financials;

        private IGenericRepository<Status> _Statuses;

        private IGenericRepository<Gallery> _Galleries;
        private IGenericRepository<RoomGallery> _RoomGalleries;

        private IGenericRepository<Inventory> _Inventories;
        private IGenericRepository<RoomInventory> _RoomInventories;
        private IGenericRepository<MoneyPushed> _MoneyPusheds;

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
        public IGenericRepository<Employee> Employees => _Employees ??= new GenericRepository<Employee>(_context);
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
        public IGenericRepository<UserType> UserTypes => _UserTypes ??= new GenericRepository<UserType>(_context);

        public IGenericRepository<Contract> Contracts => _Contracts ??= new GenericRepository<Contract>(_context);
        public IGenericRepository<ContractSubWaste> ContractSubWastes => _ContractSubWastes ??= new GenericRepository<ContractSubWaste>(_context);
        public IGenericRepository<ContractSubProduct> ContractSubProducts => _ContractSubProducts ??= new GenericRepository<ContractSubProduct>(_context);

        public IGenericRepository<MainWaste> MainWastes => _MainWastes ??= new GenericRepository<MainWaste>(_context);
        public IGenericRepository<SubWaste> SubWastes => _SubWastes ??= new GenericRepository<SubWaste>(_context);

        public IGenericRepository<MainProduct> MainProducts => _MainProducts ??= new GenericRepository<MainProduct>(_context);
        public IGenericRepository<SubProduct> SubProducts => _SubProducts ??= new GenericRepository<SubProduct>(_context);

        public IGenericRepository<OrderBuyFromClient> OrderBuyFromClients => _OrderBuyFromClients ??= new GenericRepository<OrderBuyFromClient>(_context);
        public IGenericRepository<OrderBuyFromClientAttachment> OrderBuyFromClientAttachments => _OrderBuyFromClientAttachments ??= new GenericRepository<OrderBuyFromClientAttachment>(_context);
        public IGenericRepository<OrderSellToFactory> OrderSellToFactorys => _OrderSellToFactories ??= new GenericRepository<OrderSellToFactory>(_context);
        public IGenericRepository<OrderSellToFactoryAttachment> OrderSellToFactoryAttachments => _OrderSellToFactoryAttachments ??= new GenericRepository<OrderSellToFactoryAttachment>(_context);

        public IGenericRepository<OrderBuyFromFactory> OrderBuyFromFactorys => _OrderBuyFromFactories ??= new GenericRepository<OrderBuyFromFactory>(_context);
        public IGenericRepository<OrderBuyFromFactoryAttachment> OrderBuyFromFactoryAttachments => _OrderBuyFromFactoryAttachments ??= new GenericRepository<OrderBuyFromFactoryAttachment>(_context);

        public IGenericRepository<OrderSellToClient> OrderSellToClients => _OrderSellToClients ??= new GenericRepository<OrderSellToClient>(_context);
        public IGenericRepository<OrderSellToClientAttachment> OrderSellToClientAttachments => _OrderSellToClientAttachments ??= new GenericRepository<OrderSellToClientAttachment>(_context);

        public IGenericRepository<Financial> Financials => _Financials ??= new GenericRepository<Financial>(_context);

        public IGenericRepository<Status> Statuses => _Statuses ??= new GenericRepository<Status>(_context);

        public IGenericRepository<Gallery> Galleries => _Galleries ??= new GenericRepository<Gallery>(_context);
        public IGenericRepository<RoomGallery> RoomGalleries => _RoomGalleries ??= new GenericRepository<RoomGallery>(_context);

        public IGenericRepository<Inventory> Inventories => _Inventories ??= new GenericRepository<Inventory>(_context);
        public IGenericRepository<RoomInventory> RoomInventories => _RoomInventories ??= new GenericRepository<RoomInventory>(_context);
        public IGenericRepository<MoneyPushed> MoneyPusheds => _MoneyPusheds ??= new GenericRepository<MoneyPushed>(_context);



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
