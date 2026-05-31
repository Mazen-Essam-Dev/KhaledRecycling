using Domain.Entities;
using Domain.Entities.Contract;
using Domain.Entities.Employees;
using Domain.Entities.Gallary;
using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Domain.Entities.SalaryManage;
using Domain.Entities.Waste;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
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

   
    public DbSet<City> Cities { get; set; }
  
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }
   
    public DbSet<Signature> Signatures { get; set; }
  
    public DbSet<SalaryManagement> SalaryManagements { get; set; }
    public DbSet<SalaryManagementAttachment> SalaryManagementAttachments { get; set; }
    
    public DbSet<UserNotification> UserNotifications { get; set; }


    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractSubWaste> ContractSubWastes { get; set; }
    public DbSet<ContractSubProduct> ContractSubProducts { get; set; }

    public DbSet<MainWaste> MainWastes { get; set; }
    public DbSet<SubWaste> SubWastes { get; set; }

    public DbSet<MainProduct> MainProducts { get; set; }
    public DbSet<SubProduct> SubProducts { get; set; }

    public DbSet<OrderBuyFromClient> OrderBuyFromClients { get; set; }
    public DbSet<OrderBuyFromClientAttachment> OrderBuyFromClientAttachments { get; set; }
    public DbSet<OrderSellToFactory> OrderSellToFactories { get; set; }
    public DbSet<OrderSellToFactoryAttachment> OrderSellToFactoryAttachments { get; set; }

    public DbSet<OrderBuyFromFactory> OrderBuyFromFactories { get; set; }
    public DbSet<OrderBuyFromFactoryAttachment> OrderBuyFromFactoryAttachments { get; set; }

    public DbSet<OrderSellToClient> OrderSellToClients { get; set; }
    public DbSet<OrderSellToClientAttachment> OrderSellToClientAttachments { get; set; }

    public DbSet<Financial> Financials { get; set; }
    public DbSet<Status> Statuses { get; set; }

    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<RoomGallery> RoomGalleries { get; set; }

    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<RoomInventory> RoomInventories { get; set; }

    public DbSet<UserPoints> UserPointss { get; set; }
    public DbSet<MoneyPushed> MoneyPusheds { get; set; }


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


      
        modelBuilder.Entity<Signature>()
       .HasOne<ApplicationUser>() // no nav property
       .WithMany()
       .HasForeignKey(t => t.UserId)
       .OnDelete(DeleteBehavior.Restrict); // or Cascade, if desired

  

        // ✅ Uniqe Constrains (EmployeeId + Month + Year)
        modelBuilder.Entity<SalaryManagement>()
            .HasIndex(s => new { s.EmployeeId, s.Month, s.Year })
            .IsUnique();

     

        modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.RoleNumber)
                .HasDefaultValue(1);


        modelBuilder.Entity<OrderBuyFromClient>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.FKUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderSellToClient>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.FKUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderSellToFactory>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.FKUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderBuyFromFactory>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.FKUserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<UserNotification>()
        .HasKey(un => new { un.UserId, un.NotificationId });

        modelBuilder.Entity<UserNotification>()
            .HasOne<ApplicationUser>()           // 👈 No navigation in model, but tell EF the type
            .WithMany()                        // No navigation back to User
            .HasForeignKey(un => un.UserId)
            .OnDelete(DeleteBehavior.Cascade);

     

        //// Supplier Category: seed two categories and default existing suppliers to category 1
        //modelBuilder.Entity<SupplierCategory>()
        //    .HasData(
        //        new SupplierCategory { Id = 1, NameAr = "عام", NameEn = "General" },
        //        new SupplierCategory { Id = 2, NameAr = "خاص", NameEn = "Special" }
        //    );


    }



}
