using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace PwCTools.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Role Count")]
        public int RoleCount
        {
            get
            {
                return Roles.Count;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer(new DAL.IdentityInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<PwCTools.Models.ProjectUser> ProjectUsers { get; set; }

        public System.Data.Entity.DbSet<PwCTools.Models.Project> Projects { get; set; }

        //public System.Data.Entity.DbSet<PwCTools.Models.ApplicationUser> ApplicationUsers { get; set; }
    }

    public static class GenericPrincipalExtensions
    {
        public static string FullName(this System.Security.Principal.IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {

                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "FullName")
                            return claim.Value;
                    }
                }
                return "";
            }
            else
                return "";
        }
    }
}