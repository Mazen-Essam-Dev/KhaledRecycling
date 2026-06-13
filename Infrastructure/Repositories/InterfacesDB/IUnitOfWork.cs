using Domain.Entities;
using Domain.Entities.Contract;
using Domain.Entities.Employees;
using Domain.Entities.Gallery;
using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Domain.Entities.SalaryManage;
using Domain.Entities.Waste;
using Infrastructure.Identity;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.InterfacesDB
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<Employee> Employees { get; }
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


        // Users
        IGenericRepository<UserType> UserTypes { get; }

        // Contracts
        IGenericRepository<Contract> Contracts { get; }
        IGenericRepository<ContractSubWaste> ContractSubWastes { get; }
        IGenericRepository<ContractSubProduct> ContractSubProducts { get; }

        // Waste
        IGenericRepository<MainWaste> MainWastes { get; }
        IGenericRepository<SubWaste> SubWastes { get; }

        // Products
        IGenericRepository<MainProduct> MainProducts { get; }
        IGenericRepository<SubProduct> SubProducts { get; }

        // Orders
        IGenericRepository<OrderBuyFromClient> OrderBuyFromClients { get; }
        IGenericRepository<OrderBuyFromClientAttachment> OrderBuyFromClientAttachments { get; }
        IGenericRepository<OrderSellToFactory> OrderSellToFactorys { get; }
        IGenericRepository<OrderSellToFactoryAttachment> OrderSellToFactoryAttachments { get; }

        IGenericRepository<OrderBuyFromFactory> OrderBuyFromFactorys { get; }
        IGenericRepository<OrderBuyFromFactoryAttachment> OrderBuyFromFactoryAttachments { get; }

        IGenericRepository<OrderSellToClient> OrderSellToClients { get; }
        IGenericRepository<OrderSellToClientAttachment> OrderSellToClientAttachments { get; }


        // Financial
        IGenericRepository<Financial> Financials { get; }

        // Status
        IGenericRepository<Status> Statuses { get; }

        // Gallery
        IGenericRepository<Gallery> Galleries { get; }
        IGenericRepository<RoomGallery> RoomGalleries { get; }

        // Inventory
        IGenericRepository<Inventory> Inventories { get; }
        IGenericRepository<RoomInventory> RoomInventories { get; }

        IGenericRepository<MoneyPushed> MoneyPusheds { get; }





        // 👇 Add this property to expose DbContext
        ApplicationDbContext Context { get; }
        Task<int> CompleteAsync(); // SaveChanges
    }
}
