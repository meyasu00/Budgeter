namespace Budgeter.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using Models.CodeFirst;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budgeter.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "SuperUser"))
            {
                roleManager.Create(new IdentityRole { Name = "SuperUser" });
            }


            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "meyasu@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "meyasu@coderfoundry.com",
                    Email = "meyasu@coderfoundry.com",
                    FirstName = "Mekuanent",
                    LastName = "Eyasu",
                }, "Abc&123");
            }
            var userId = userManager.FindByEmail("meyasu@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Admin");

            context.Warnings.AddOrUpdate(x => x.WarningLevel,
                new Warning() { WarningLevel = "None" },
                new Warning() { WarningLevel = "50" },
                new Warning() { WarningLevel = "100" },
                new Warning() { WarningLevel = "250" },
                new Warning() { WarningLevel = "500" },
                new Warning() { WarningLevel = "750" },
                new Warning() { WarningLevel = "1000" }
                );

            context.CategoryStandards.AddOrUpdate(x => x.Name,
                new CategoryStandard() { Name = "Household" },
                new CategoryStandard() { Name = "Food" },
                new CategoryStandard() { Name = "Health/Medical" },
                new CategoryStandard() { Name = "Transportation" },
                new CategoryStandard() { Name = "Taxes/Fees" },
                new CategoryStandard() { Name = "Salary" },
                new CategoryStandard() { Name = "Hobbies/Leisure" },
                new CategoryStandard() { Name = "Miscellaneous" }
                );
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
