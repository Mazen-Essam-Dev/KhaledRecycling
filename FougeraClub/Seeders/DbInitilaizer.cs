using Application.Helpers;
using CsvHelper.Configuration;
using Domain.Entities;
using Domain.Enums;
using KhaledTeamRecycling.Helpers;
using Humanizer.Localisation;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using CsvReader = CsvHelper.CsvReader;

namespace KhaledTeamRecycling.Seeders
{
    public static class DbInitilaizer
    {

        public static async Task Initialize(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            using var context = new ApplicationDbContext(
               serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var config = serviceProvider.GetRequiredService<IConfiguration>();

            context.Database.EnsureCreated();

            if (!context.Nationalities.Any())
            {
                var filePath = Path.Combine(env.WebRootPath, "SeedData", "Country-Table.csv");

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    var nationalities = csv.GetRecords<Nationality>()
                        .Select(n => new Nationality
                        {
                            NameAr = n.NameAr,
                            NameEn = n.NameEn
                        }).ToList();

                    await context.Nationalities.AddRangeAsync(nationalities);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Nationalities.Any())
            {
                var filePath = Path.Combine(env.WebRootPath, "SeedData", "Country-Table.csv");

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    var nationalities = csv.GetRecords<Nationality>()
                        .Select(n => new Nationality
                        {
                            NameAr = n.NameAr,
                            NameEn = n.NameEn
                        }).ToList();

                    await context.Nationalities.AddRangeAsync(nationalities);
                    await context.SaveChangesAsync();
                }
            }

            ////// Seed ReportType Enum Data
            ////if (!context.ReportTypes.Any())
            ////{
            ////    var list = Enum.GetValues(typeof(ReportTypeEnum))
            ////        .Cast<ReportTypeEnum>()
            ////        .Select(e => new ReportType
            ////        {
            ////            Id = (int)e,
            ////            NameEn = Domain.Resources.Resource1.ResourceManager.GetString(e.GetDisplayKey(), new CultureInfo("en")),
            ////            NameAr = Domain.Resources.Resource1.ResourceManager.GetString(e.GetDisplayKey(), new CultureInfo("ar"))
            ////        })
            ////        .ToList();

            ////    await context.ReportTypes.AddRangeAsync(list);
            ////    await context.SaveChangesAsync();
            ////}




            // Seed ExpensesSource Enum Data
            if (!context.MemberTypes.Any())
            {
                var list = Enum.GetValues(typeof(MemberTypeEnum))
                    .Cast<MemberTypeEnum>()
                    .Select(e => new MemberType
                    {
                        Id = (int)e,
                        NameEn = Domain.Resources.Resource2.ResourceManager.GetString(e.GetDisplayKey(), new CultureInfo("en")),
                        NameAr = Domain.Resources.Resource2.ResourceManager.GetString(e.GetDisplayKey(), new CultureInfo("ar"))
                    })
                    .ToList();

                await context.MemberTypes.AddRangeAsync(list);
                await context.SaveChangesAsync();
            }

            // Get all Permissions Of This Application System
            var allPermissions = PermissionScanner.GetAllActionPermissions();

            #region adding Role "SuperAdmin" // with All Permission
            // adding Role "SuperAdmin" // with All Permission
            var roleNameSuper = Role.SuperAdmin.ToString() /*"SuperAdmin"*/;
            var roleSuper = await roleManager.FindByNameAsync(roleNameSuper);
            if (roleSuper == null)
            {
                roleSuper = new ApplicationRole { Name = roleNameSuper };
                await roleManager.CreateAsync(roleSuper);
            }

            if (roleSuper == null)
                throw new Exception("SuperAdmin role not found");

            var existingClaimsForSuperAdmin = await roleManager.GetClaimsAsync(roleSuper);

            foreach (var permission in allPermissions)
            {
                if (!existingClaimsForSuperAdmin.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(roleSuper, new Claim("Permission", permission));
                }
            }
            #endregion adding Role "SuperAdmin" // with All Permission

            #region adding Role "Master" // with All Permission
            // adding Role "Master" // with All Permission
            var roleNameMaster = Role.Master.ToString() /*"Master"*/;
            var roleMaster = await roleManager.FindByNameAsync(roleNameMaster);
            if (roleMaster == null)
            {
                roleMaster = new ApplicationRole { Name = roleNameMaster };
                await roleManager.CreateAsync(roleMaster);
            }

            if (roleMaster == null)
                throw new Exception("Master role not found");

            var existingClaimsForMaster = await roleManager.GetClaimsAsync(roleMaster);

            foreach (var permission in allPermissions)
            {
                if (!existingClaimsForMaster.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(roleMaster, new Claim("Permission", permission));
                }
            }
            #endregion adding Role "Master" // with All Permission

            if (!context.Departments.Any())
            {
                context.Departments.Add(new Department { NameAr = $"قسم تقنية المعلومات والبرمجة", NameEn = $"Department of Information Technology and Programming" });
                context.Departments.Add(new Department { NameAr = $"قسم الذكاء االأصطناعى والروبوت", NameEn = $"Department of Artificial Intelligence and Robotics" });
                context.Departments.Add(new Department { NameAr = $"القسم الصناعى والمهنى", NameEn = $"Industrial and Professional Department" });
                context.Departments.Add(new Department { NameAr = $"قسم التصميم والتنفيذ", NameEn = $"Design and Implementation Department" });
                context.Departments.Add(new Department { NameAr = $"قسم الطيران وعلوم الفضاء", NameEn = $"Department of Aviation and Space Sciences" });

                await context.SaveChangesAsync();
            }

            //if (!context.Users.Any())
            //{
                #region Create the "SuperAdmin" User And Pass His Role
                //Seed the DB (Users table) with 1 Admin user
                var adminUser = new ApplicationUser
                {
                    FullNameAr = "سوبر ادمن",
                    FullNameEn = "superAdmin",
                    UserName = "superAdmin",
                    Email = "superAdmin@test.com",
                    EmailConfirmed = true,
                };

                // Check if the userSuper doesn't exist in BD => Add it 
                var userSuper = await userManager.FindByEmailAsync(adminUser.Email);
                if (userSuper is null)
                {
                    var adminPass = config["Admin:Password"]; // must be set in appsettings.json or secrets

                    await userManager.CreateAsync(adminUser, adminPass);
                    //add the user to all roles (becasue the super admin has all the permissions)
                    await userManager.AddToRoleAsync(adminUser, Role.SuperAdmin.ToString());
                }
                #endregion Create the SuperAdmin User And Pass His Role

                #region Create the "Master" User And Pass His Role
                //Seed the DB (Users table) with 1 Admin user
                var masterUser = new ApplicationUser
                {
                    FullNameAr = "ماستر",
                    FullNameEn = "Master",
                    UserName = "Master",
                    Email = "Master@test.com",
                    EmailConfirmed = true,
                };

                // Check if the userSuper doesn't exist in BD => Add it 
                var ExistuserMaster = await userManager.FindByEmailAsync(masterUser.Email);
                if (ExistuserMaster is null)
                {
                    var masterPass = config["Admin:MasterPassword"]; // must be set in appsettings.json or secrets

                    await userManager.CreateAsync(masterUser, masterPass);
                    //add the user to all roles (becasue the super admin has all the permissions)
                    await userManager.AddToRoleAsync(masterUser, Role.Master.ToString());
                }
                #endregion Create the SuperAdmin User And Pass His Role

                if (!context.Cities.Any())
                {
                    var filePath = Path.Combine(env.WebRootPath, "SeedData", "City-Table.csv");

                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        var cities = csv.GetRecords<City>()
                            .Select(n => new City
                            {
                                NameAr = n.NameAr,
                                NameEn = n.NameEn
                            }).ToList();

                        await context.Cities.AddRangeAsync(cities);
                    }

                    await context.SaveChangesAsync();
                }
            //}


            if (!context.Members.Any())
            {
                var memberPass = config["Member:Password"]; // must be set in appsettings.json or secrets
                                                            // Seed the Member user
                var defaultMemberUser = new MemberEntity
                {
                    Code = 1000,
                    Email = "member@test.com",
                    AcademicQualification = "Bachelor's Degree",
                    Address = "123 Main St",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Age = 33,
                    CityId = 1, // Assuming a valid city ID exists
                    FullNameAr = "جون دو",
                    FullNameEn = "John Doe",
                    EducationInstitution = "University of Example",
                    Facebook = "https://facebook.com/johndoe",
                    PhoneNumber = "1234567890",
                    Profession = "Software Engineer",
                    Languages = "English, Arabic",
                    NationalityId = context.Nationalities.OrderByDescending(n => n.Id).Last().Id, //
                    GenderId = 1, // Assuming
                    Hobby = "Reading, Coding",
                    IdNumber = "ID123456",
                    IdExpiryDate = new DateOnly(2025, 12, 31),
                    HeardBy = 1,
                    IdImagePath = "path/to/id/image.jpg",
                    PassportImagePath = "path/to/passport/image.jpg",
                    Instagram = "https://instagram.com/johndoe",
                    Xplatform = "https://twitter.com/johndoe",
                    License = true,
                    Password = HashHelper.ComputeSha256Hash(memberPass)
                };

                await context.AddAsync(defaultMemberUser);

                await context.SaveChangesAsync();

            }

        }
    }
}