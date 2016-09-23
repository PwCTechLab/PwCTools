using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PwCTools.DAL;
using PwCTools.Models;
using PwCTools.DAL.Repositories;
using PagedList;

namespace PwCTools.Controllers
{
    public class SprintsController : Controller
    {
        private ISprintRepository sprintRepository;

        public SprintsController()
        {
            this.sprintRepository = new SprintRepository(new ApplicationDbContext());
        }

        public SprintsController(ISprintRepository sprintRepository)
        {
            this.sprintRepository = sprintRepository;
        }

        //
        // GET: /Sprint/
        [Authorize(Roles = "Administrators")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var sprints = from s in sprintRepository.GetSprints()
                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                sprints = sprints.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    sprints = sprints.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    sprints = sprints.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    sprints = sprints.OrderByDescending(s => s.Description);
                    break;
                default:  // Name ascending 
                    sprints = sprints.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(sprints.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Sprint/Details/5
        [Authorize(Roles = "Administrators")]
        public ViewResult Details(int id)
        {
            Sprint sprint = sprintRepository.GetSprintByID(id);
            return View(sprint);
        }

        //
        // GET: /Sprint/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            ViewBag.ProjectList = new SelectList(new ProjectRepository(new ApplicationDbContext()).GetProjects(), "Id", "Name");
            return View();
        }

        // POST: Sprints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Name,Description,StartDate,EndDate,IsActive")] Sprint sprint)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sprintRepository.InsertSprint(sprint);
                    sprintRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(sprint);
        }

        //
        // GET: /Sprint/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            Sprint sprint = sprintRepository.GetSprintByID(id);

            //Set project select
            ViewBag.ProjectList = new SelectList(new ProjectRepository(new ApplicationDbContext()).GetProjects(), "Id", "Name", sprint.ProjectId);

            return View(sprint);
        }

        // POST: Sprints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,Name,Description,StartDate,EndDate,IsActive")] Sprint sprint)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sprintRepository.UpdateSprint(sprint);
                    sprintRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(sprint);
        }

        //
        // GET: /Sprint/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Sprint sprint = sprintRepository.GetSprintByID(id);
            return View(sprint);
        }

        // POST: Sprints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Sprint sprint = sprintRepository.GetSprintByID(id);
                sprintRepository.DeleteSprint(id);
                sprintRepository.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            sprintRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
