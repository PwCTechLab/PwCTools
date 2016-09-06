using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PwCTools.DAL
{
    public class IdentityInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {

            if (!db.Users.Any())
            {
                // Add missing roles
                CreateRole(db, "Administrators");
                CreateRole(db, "User");

                // Create test users
                CreateUser(db, "admin", "Admin", "User", "xxx@us.pwc.com", "5551234567", "Administrators");
                CreateUser(db, "csallee002", "Chris", "Sallee", "christopher.sallee@us.pwc.com", "2028157612", "User");
                CreateUser(db, "MHall", "Mike", "Hall", "michael.p.hall@us.pwc.com", "7038014959", "User");

            }
        }

        private static void CreateRole(ApplicationDbContext db, string roleName)
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                roleManager.Create(role);
            }
        }

        private static void CreateUser(ApplicationDbContext db, string userName, string firstName, string lastName, string email, string phoneNumber, string role)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var user = userManager.FindByName(userName);
            if (user == null)
            {
                var newUser = new ApplicationUser()
                {
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    //MustChangePassword = false
                };
                userManager.Create(newUser, "Wer!2345");
                userManager.SetLockoutEnabled(newUser.Id, false);
                userManager.AddToRole(newUser.Id, role);
            }
        }
    }
}