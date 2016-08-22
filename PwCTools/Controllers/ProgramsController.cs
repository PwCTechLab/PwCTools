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
    public class ProgramsController : Controller
    {
        private IProgramRepository programRepository;

        public ProgramsController()
        {
            this.programRepository = new ProgramRepository(new BoardContext());
        }

        public ProgramsController(IProgramRepository programRepository)
        {
            this.programRepository = programRepository;
        }

        //
        // GET: /Program/
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

            var programs = from s in programRepository.GetPrograms()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                programs = programs.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    programs = programs.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    programs = programs.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    programs = programs.OrderByDescending(s => s.Description);
                    break;
                default:  // Name ascending 
                    programs = programs.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(programs.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Program/Details/5
        [Authorize(Roles = "Administrators")]
        public ViewResult Details(int id)
        {
            Program program = programRepository.GetProgramByID(id);
            return View(program);
        }

        //
        // GET: /Program/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Program program)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    programRepository.InsertProgram(program);
                    programRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(program);
        }

        //
        // GET: /Program/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            Program program = programRepository.GetProgramByID(id);
            return View(program);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Program program)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    programRepository.UpdateProgram(program);
                    programRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(program);
        }

        //
        // GET: /Program/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Program program = programRepository.GetProgramByID(id);
            return View(program);
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Program program = programRepository.GetProgramByID(id);
                programRepository.DeleteProgram(id);
                programRepository.Save();
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
            programRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
