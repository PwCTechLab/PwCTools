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

namespace PwCTools.Controllers
{
    public class ProjectUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectUsers
        [Authorize(Roles = "Administrators")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ProjectSortParm = sortOrder == "Project" ? "project_desc" : "Project";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var projectUsers = db.ProjectUsers.Include(p => p.Project).Include(p => p.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                projectUsers =  projectUsers.ToList()
                    .Where(s => s.User.FullName.ToUpper().Contains(searchString.ToUpper())).AsQueryable();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    projectUsers = projectUsers.OrderByDescending(l => l.User.LastName).ThenByDescending(f => f.User.FirstName);
                    break;
                case "Project":
                    projectUsers = projectUsers.OrderBy(s => s.Project.Name);
                    break;
                case "project_desc":
                    projectUsers = projectUsers.OrderByDescending(s => s.Project.Name);
                    break;
                default:  // Name ascending 
                    projectUsers = projectUsers.OrderBy(l => l.User.LastName).ThenBy(f => f.User.FirstName);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(projectUsers.ToPagedList(pageNumber, pageSize));
        }

        // GET: ProjectUsers/Details/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            if (projectUser == null)
            {
                return HttpNotFound();
            }
            return View(projectUser);
        }

        // GET: ProjectUsers/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            ViewBag.ProjectList = new SelectList(db.Projects, "Id", "Name");
            ViewBag.UserList = new SelectList(db.Users, "Id", "FullName");
            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ProjectId,Default")] ProjectUser projectUser)
        {
            if (ModelState.IsValid)
            {
                db.ProjectUsers.Add(projectUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projectUser);
        }

        // GET: ProjectUsers/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            if (projectUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectList = new SelectList(db.Projects, "Id", "Name", projectUser.ProjectId);
            ViewBag.UserList = new SelectList(db.Users, "Id", "FullName", projectUser.UserId);
            return View(projectUser);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ProjectId,Default")] ProjectUser projectUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectUser);
        }

        // GET: ProjectUsers/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            if (projectUser == null)
            {
                return HttpNotFound();
            }
            return View(projectUser);
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            db.ProjectUsers.Remove(projectUser);
            db.SaveChanges();
            return RedirectToAction("Index");
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
