using Domain.Entities;
using Domain.Entities.Employees;
using Domain.Entities.SalaryManage;
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

   
    public DbSet<City> Cities { get; set; }
    public DbSet<Department> Departments { get; set; }
  
    public DbSet<Employee> Employees { get; set; }
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<MemberType> MemberTypes { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }
   
    public DbSet<Signature> Signatures { get; set; }
  
    public DbSet<SalaryManagement> SalaryManagements { get; set; }
    public DbSet<SalaryManagementAttachment> SalaryManagementAttachments { get; set; }
    
    public DbSet<UserNotification> UserNotifications { get; set; }
   

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
