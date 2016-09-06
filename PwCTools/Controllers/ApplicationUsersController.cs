using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PwCTools.Models;
using PagedList;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace PwCTools.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        [Authorize(Roles = "Administrators")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastname_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var users = db.Users.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.ToList()
                    .Where(s => s.FullName.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            switch (sortOrder)
            {
                case "lastname_desc":
                    users = users.OrderByDescending(l => l.LastName).ThenByDescending(f => f.FirstName).ToList();
                    break;
                case "FirstName":
                    users = users.OrderBy(s => s.FirstName).ToList();
                    break;
                case "firstname_desc":
                    users = users.OrderByDescending(s => s.FirstName).ToList();
                    break;
                default:  // Name ascending 
                    users = users.OrderBy(l => l.LastName).ThenBy(f => f.FirstName).ToList();
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            ViewBag.RolesList = new MultiSelectList(roleManager.Roles, "Name", "Name");
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,Email,PhoneNumber,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = userManager.FindByName(applicationUser.UserName);
                if (user == null)
                {
                    userManager.Create(applicationUser);
                    userManager.SetLockoutEnabled(applicationUser.Id, false);
                    userManager.AddToRoles(applicationUser.Id, Request["Role"]);

                    return RedirectToAction("Index");
                }
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            ViewBag.RolesList = new MultiSelectList(roleManager.Roles, "Name", "Name", userManager.GetRoles(applicationUser.Id));
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FirstName,LastName,Email,PhoneNumber,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                string userId = userManager.FindByName(applicationUser.UserName).Id;

                userManager.RemoveFromRoles(userId, userManager.GetRoles(userId).ToArray());
                userManager.AddToRoles(userId, Request["Role"].Split(','));

                userManager.Update(applicationUser);

                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
