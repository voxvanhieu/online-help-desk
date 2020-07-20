using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineHelpDesk.Models;
using OnlineHelpDesk.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    public class DatabaseHelper
    {
        public static async Task InitializeRequiredData()
        {
            using (var dbSeeder = new DatabaseSeeder())
            {
                dbSeeder.SeedRolesAndAdmin();
                await dbSeeder.SeedRequestType();
                await dbSeeder.SeedStatusType();
            }
        }

        public static async Task SeedData()
        {
            await Task.Run(() =>
            {

                using (var dbSeeder = new DatabaseSeeder())
                {
                    dbSeeder.SeedUsers().Wait();
                    dbSeeder.SeedFacility().Wait();
                    dbSeeder.SeedEquipmentType().Wait();
                    dbSeeder.SeedEquipment().Wait();
                }
            });
        }
    }
}

public class DatabaseSeeder : IDisposable
{
    private ApplicationDbContext context;
    private ApplicationDbContext DbContext
    {
        get
        {
            //context ??= ApplicationDbContext.Create();
            if (context == null) context = ApplicationDbContext.Create();
            return context;
        }

        set => context = value;
    }
    //public UserService(ApplicationDbContext db) => this.db = db;

    public async Task SeedUsers()
    {
        await Task.Run(() =>
        {
            using (var userService = new UserService())
            {
                _ = userService.CreateUser(new ApplicationUser()
                {
                    UserName = "ST000001",
                    UserIdentityCode = "ST000001",
                    FullName = "Sample Student",
                    Email = "student@ohd.com"
                }, role: "Student");

                _ = userService.CreateUser(new ApplicationUser()
                {
                    UserName = "AS000001",
                    UserIdentityCode = "AS000001",
                    FullName = "Sample Assignor",
                    Email = "assignor@ohd.com"
                }, role: "Assignor");

                _ = userService.CreateUser(new ApplicationUser()
                {
                    UserName = "FH000001",
                    UserIdentityCode = "FH000001",
                    FullName = "Sample Facility Head",
                    Email = "f.head@ohd.com"
                }, role: "FacilityHead");
            }
        });
    }

    public async Task SeedFacility()
    {
        if (DbContext.Facilities.Any()) return;
        using (var transaction = DbContext.Database.BeginTransaction())
        {
            try
            {
                DbContext.Facilities.AddOrUpdate(x => x.Name,
                    new Facility { Name = "Classroom", CreatedAt = DateTime.Now },
                    new Facility { Name = "Lab-Assistants", CreatedAt = DateTime.Now },
                    new Facility { Name = "Hostels", CreatedAt = DateTime.Now },
                    new Facility { Name = "Canteen", CreatedAt = DateTime.Now },
                    new Facility { Name = "Gymnasium", CreatedAt = DateTime.Now },
                    new Facility { Name = "Computer Centre", CreatedAt = DateTime.Now },
                    new Facility { Name = "Faculty Club", CreatedAt = DateTime.Now },
                    new Facility { Name = "Others", CreatedAt = DateTime.Now }
                    );

                await DbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }

    public async Task SeedEquipmentType()
    {
        if (DbContext.EquipmentTypes.Any()) return;
        using (var transaction = DbContext.Database.BeginTransaction())
        {
            try
            {
                List<string> equipmentTypes = new List<string>()
                        {
                            "Treadmill",
                            "Dumbbells",
                            "Exercise Bike",
                            "Elliptical",
                            "Kettelbell",
                            "Medicine Ball",
                            "Rowing Machine",
                            "Punching Bag",
                            "Bar",
                            "Trampoline",
                            "Jump Rope",
                            "Blackboard",
                            "Computer",
                            "Clock",
                            "Globe",
                            "File holder",
                            "Map",
                            "Chair",
                            "Desk",
                            "Laptop",
                            "Cellings lights",
                            "Projector",
                            "Wireless",
                            "Air-conditioner",
                            "Cellings fans",
                            "Lamp",
                            "Shower",
                            "Bed",
                            "Mirror",
                            "Door",
                            "Window",
                            "Toiletries",
                            "Monitor",
                            "Case",
                            "Motherboard",
                            "Memory",
                            "Mouse",
                            "Speaker",
                            "Keyboard",
                            "Cable",
                            "Microphone",
                            "Modem",
                            "Webcam",
                            "CD-ROM drive",
                            "CPU",
                            "Hard drive",
                            "USB drive",
                            "Water cooler",
                            "Refrigerator",
                            "Vacuum cleaner",
                            "Appliance plug",
                            "Evaporative cooler",
                            "Washing machine",
                            "Remote",
                            "Pipette",
                            "Test tube rack",
                            "Test tube",
                            "Erlenmeyer flask",
                            "Beaker",
                            "Bunsen burner",
                            "Barometer",
                            "Indicator",
                            "Stopwatch",
                            "Speedometer",
                            "Protractor",
                            "Alcohol burner",
                            "Syringe",
                            "Graduated cylinder",
                            "Dropper",
                            "Tongs",
                            "Turing fork",
                            "Stergoscope",
                            "Magnet",
                            "Magnifiers",
                            "Telescope",
                            "Microscope",
                            "Thermometer",
                            "Friability tester",
                            "Pulleys",
                            "Tape",
                            "Dry-cell batery",
                            "Level",
                            "Balance scale",
                            "Chart",
                            "Dissecting set",
                            "Earth science",
                            "Printer",
                            "Scanner",
                            "Fax machine"
                        };

                equipmentTypes.ForEach(typename =>
                {
                    DbContext.EquipmentTypes.AddOrUpdate(x => x.TypeName,
                        new EquipmentType { TypeName = typename });
                });

                await DbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }

    public async Task SeedEquipment()
    {
        if (DbContext.Equipments.Any()) return;
        using (var transaction = DbContext.Database.BeginTransaction())
        {
            try
            {
                // Add all ET to each Facility
                for (int i = 0; i < DbContext.Facilities.Count(); i++)
                {
                    for (int j = 0; j < DbContext.EquipmentTypes.Count(); j++)
                    {
                        //context.Equipments.AddOrUpdate(e => new { e.FacilityId, e.ArtifactId },
                        DbContext.Equipments.Add(
                            new Equipment
                            {
                                FacilityId = i + 1,
                                ArtifactId = j + 1
                            });
                    }
                }

                await DbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }


    #region Important data, called in InitializeRequiredData
    public void SeedRolesAndAdmin()
    {
        if (DbContext.Roles.Any()) return;

        // Add missing roles
        if (!RoleManager.RoleExists("SuperAdmin"))
        {
            // Create new role: Super Admin
            RoleManager.Create(new IdentityRole("SuperAdmin"));

            // Create default admin
            var newUser = new ApplicationUser()
            {
                UserName = "admin",
                FullName = "Super Admin",
                Email = "admin@ohd.com",
                CreatedAt = DateTime.UtcNow,
                MustChangePassword = true
            };
            UserManager.Create(newUser, "admin@123");
            UserManager.SetLockoutEnabled(newUser.Id, false);
            UserManager.AddToRole(newUser.Id, "SuperAdmin");
            // Note:
            // - Create new user with role SuperAdmin
            // - Default password ADM@123a
            // - He must to change password at first login
        }

        if (!RoleManager.RoleExists("Student"))
        {
            RoleManager.Create(new IdentityRole("Student"));
        }

        if (!RoleManager.RoleExists("Assignor"))
        {
            RoleManager.Create(new IdentityRole("Assignor"));
        }

        if (!RoleManager.RoleExists("FacilityHead"))
        {
            RoleManager.Create(new IdentityRole("FacilityHead"));
        }
    }

    public async Task SeedRequestType()
    {
        if (DbContext.RequestTypes.Any()) return;
        using (var transaction = DbContext.Database.BeginTransaction())
        {
            try
            {
                DbContext.RequestTypes.AddOrUpdate(x => x.TypeName,
                    new RequestType { TypeName = "Q&A" },
                    new RequestType { TypeName = "Report broken equipment" },
                    new RequestType { TypeName = "Additional equipment required" }
                    );

                await DbContext.SaveChangesAsync();
                //DbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }

    public async Task SeedStatusType()
    {
        if (DbContext.StatusTypes.Any()) return;
        using (var transaction = DbContext.Database.BeginTransaction())
        {
            try
            {
                DbContext.StatusTypes.AddOrUpdate(x => x.TypeName,
                    new StatusType { TypeName = "Created" },
                    new StatusType { TypeName = "Assigned" },
                    new StatusType { TypeName = "Processing" },
                    new StatusType { TypeName = "Completed" },
                    new StatusType { TypeName = "Closed" }
                    );

                await DbContext.SaveChangesAsync();
                //DbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
    #endregion

    #region Helpers
    private UserManager<ApplicationUser> _userManager;
    public UserManager<ApplicationUser> UserManager
    {
        get => _userManager ?? new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
        private set => _userManager = value;
    }

    private RoleManager<IdentityRole> _roleManager;
    public RoleManager<IdentityRole> RoleManager
    {
        get => _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DbContext));
        private set => _roleManager = value;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _userManager?.Dispose();
            _userManager = null;

            _roleManager?.Dispose();
            _roleManager = null;

            // coalescing statement in C# 8.0 :))
            //db ??= null;
            DbContext?.Dispose();
            DbContext = null;
        }
    }
    #endregion

}