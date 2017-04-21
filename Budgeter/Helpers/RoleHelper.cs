using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public static class RoleHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void AddSuperUser(this string userId)
        {
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            userManager.AddToRole(userId, "SuperUser");
        }

        public static void RemoveSuperUser(this string userId)
        {
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            userManager.RemoveFromRole(userId, "SuperUser");
        }

        public static void AddUserToAdmin(this string userId)
        {
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            userManager.AddToRole(userId, "Admin");
        }

        public static void RemoveUserFromAdmin(this string userId)
        {
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            userManager.RemoveFromRole(userId, "Admin");
        }
    }
}