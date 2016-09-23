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
                if (db.Programs.Count() == 0)
                {
                    var programs = new List<Program> { new Program { Name="FIMA", Description="Federal Insurance Mitigation Administration", IsActive=true }};

                    programs.ForEach(s => db.Programs.Add(s));
                    db.SaveChanges();
                }

                if (db.Projects.Count() == 0)
                {
                    var projects = new List<Project> { new Project { ProgramId=1, Name="UCORT", Description="UCORT Project", IsActive=true }};

                    projects.ForEach(s => db.Projects.Add(s));
                    db.SaveChanges();
                }

                // Add missing roles
                CreateRole(db, "Administrators");
                CreateRole(db, "User");

                // Create test users
                CreateUser(db, "admin", "Admin", "User", "xxx@us.pwc.com", "5551234567", "Administrators");
                CreateProjectUser(db, CreateUser(db, "csallee002", "Chris", "Sallee", "christopher.sallee@us.pwc.com", "2028157612", "User"), 1);
                CreateProjectUser(db, CreateUser(db, "MHall", "Mike", "Hall", "michael.p.hall@us.pwc.com", "7038014959", "User"), 1);

                if (db.Columns.Count() == 0)
                {
                    var columns = new List<Column>
                {
                    new Column { ProjectId=1, Name="To Do", Description="To Do Column" },
                    new Column { ProjectId=1, Name="In Progress", Description="In Progress Column" },
                    new Column { ProjectId=1, Name="Test", Description="Test Column" },
                    new Column { ProjectId=1, Name="Done", Description="Done Column" }
                };

                    columns.ForEach(s => db.Columns.Add(s));
                    db.SaveChanges();
                }

                if (db.Sprints.Count() == 0)
                {
                    var sprints = new List<Sprint>
                {
                    new Sprint { ProjectId=1, Name="Completed Sprint", Description="Completed UCORT Sprint", StartDate=DateTime.Now.AddDays(-14), EndDate=DateTime.Now, IsActive=false },
                    new Sprint { ProjectId=1, Name="Active Sprint", Description="Active UCORT Sprint", StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(14), IsActive=true }
                };

                    sprints.ForEach(s => db.Sprints.Add(s));
                    db.SaveChanges();
                }

                if (db.BoardTasks.Count() == 0)
                {
                    var tasks = new List<BoardTask>
                {
                    new BoardTask { SprintId=1, ColumnId=1, Name="Task 1", Description="Task 1 Description", CreatedDate=DateTime.Now.AddDays(-12), DueDate=DateTime.Now.AddDays(+12) },
                    new BoardTask { SprintId=2, ColumnId=1, Name="Task 2", Description="Task 2 Description", CreatedDate=DateTime.Now.AddDays(-11), DueDate=DateTime.Now.AddDays(+12) },
                    new BoardTask { SprintId=2, ColumnId=1, Name="Task 3", Description="Task 3 Description", CreatedDate=DateTime.Now.AddDays(-10), DueDate=DateTime.Now.AddDays(+12) },
                    new BoardTask { SprintId=2, ColumnId=1, Name="Task 4", Description="Task 4 Description", CreatedDate=DateTime.Now.AddDays(-9), DueDate=DateTime.Now.AddDays(+12) },
                    new BoardTask { SprintId=2, ColumnId=1, Name="Task 5", Description="Task 5 Description", CreatedDate=DateTime.Now.AddDays(-8), DueDate=DateTime.Now.AddDays(+12) }
                };

                    tasks.ForEach(s => db.BoardTasks.Add(s));
                    db.SaveChanges();
                }

                if (db.BoardTasksComments.Count() == 0)
                {
                    var store = new UserStore<ApplicationUser>(db);
                    var userManager = new UserManager<ApplicationUser>(store);

                    var comments = new List<BoardTaskComment>
                    {
                        new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 1", CreatedById=userManager.FindByName("csallee002").Id, CreatedDateTime=DateTime.Now.AddDays(-2) },
                        new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 2", CreatedById=userManager.FindByName("MHall").Id, CreatedDateTime=DateTime.Now.AddHours(-12) },
                        new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 3", CreatedById=userManager.FindByName("csallee002").Id, CreatedDateTime=DateTime.Now.AddHours(-1) }
                    };

                    comments.ForEach(s => db.BoardTasksComments.Add(s));
                    db.SaveChanges();
                }

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

        private static string CreateUser(ApplicationDbContext db, string userName, string firstName, string lastName, string email, string phoneNumber, string role)
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

                return newUser.Id;
            }
            return null;
        }

        private static void CreateProjectUser(ApplicationDbContext db, string userId, int projectId)
        {
            var projectusers = new List<ProjectUser> { new ProjectUser { UserId = userId, ProjectId = projectId, Default = true } };

            projectusers.ForEach(s => db.ProjectUsers.Add(s));
            db.SaveChanges();
        }
    }
}