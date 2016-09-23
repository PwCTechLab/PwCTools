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
    public class ProjectsController : Controller
    {
        private IProjectRepository projectRepository;

        public ProjectsController()
        {
            this.projectRepository = new ProjectRepository(new ApplicationDbContext());
        }

        public ProjectsController(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        //
        // GET: /Project/
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

            var projects = from s in projectRepository.GetProjects()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    projects = projects.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    projects = projects.OrderByDescending(s => s.Description);
                    break;
                default:  // Name ascending 
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Project/Details/5
        [Authorize(Roles = "Administrators")]
        public ViewResult Details(int id)
        {
            Project project = projectRepository.GetProjectByID(id);
            return View(project);
        }

        //
        // GET: /Program/ColumnsDetailsPartial/5
        [Authorize(Roles = "Administrators")]
        public ViewResult DetailsPartialColumns(int id, int? page)
        {
            List<Column> columns = projectRepository.GetProjectByID(id).Columns;

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(columns.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Program/SprintsDetailsPartial/5
        [Authorize(Roles = "Administrators")]
        public ViewResult DetailsPartialSprints(int id, int? page)
        {
            List<Sprint> sprints = projectRepository.GetProjectByID(id).Sprints;

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(sprints.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Project/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            ViewBag.ProgramList = new SelectList(new ProgramRepository(new ApplicationDbContext()).GetPrograms(), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProgramId,Name,Description,IsActive")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    projectRepository.InsertProject(project);
                    projectRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(project);
        }

        //
        // GET: /Project/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            Project project = projectRepository.GetProjectByID(id);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProgramId,Name,Description,IsActive")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    projectRepository.UpdateProject(project);
                    projectRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(project);
        }

        //
        // GET: /Project/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Project project = projectRepository.GetProjectByID(id);
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Project project = projectRepository.GetProjectByID(id);
                projectRepository.DeleteProject(id);
                projectRepository.Save();
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
            projectRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
