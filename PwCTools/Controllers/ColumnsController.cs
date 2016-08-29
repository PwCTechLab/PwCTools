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
    public class ColumnsController : Controller
    {
        private IColumnRepository columnRepository;

        public ColumnsController()
        {
            this.columnRepository = new ColumnRepository(new BoardContext());
        }

        public ColumnsController(IColumnRepository columnRepository)
        {
            this.columnRepository = columnRepository;
        }

        //
        // GET: /Column/
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

            var columns = from s in columnRepository.GetColumns()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                columns = columns.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    columns = columns.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    columns = columns.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    columns = columns.OrderByDescending(s => s.Description);
                    break;
                default:  // Name ascending 
                    columns = columns.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(columns.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Column/Details/5
        [Authorize(Roles = "Administrators")]
        public ViewResult Details(int id)
        {
            Column column = columnRepository.GetColumnByID(id);
            return View(column);
        }

        //
        // GET: /Column/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Create()
        {
            ViewBag.ProjectList = new SelectList(new ProjectRepository(new BoardContext()).GetProjects(), "Id", "Name");
            return View();
        }

        // POST: Columns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Name,Description")] Column column)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    columnRepository.InsertColumn(column);
                    columnRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(column);
        }

        //
        // GET: /Column/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            Column column = columnRepository.GetColumnByID(id);

            //Set project select
            ViewBag.ProjectList = new SelectList(new ProjectRepository(new BoardContext()).GetProjects(), "Id", "Name", column.ProjectId);

            return View(column);
        }

        // POST: Columns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,Name,Description")] Column column)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    columnRepository.UpdateColumn(column);
                    columnRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(column);
        }

        //
        // GET: /Column/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Column column = columnRepository.GetColumnByID(id);
            return View(column);
        }

        // POST: Columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Column column = columnRepository.GetColumnByID(id);
                columnRepository.DeleteColumn(id);
                columnRepository.Save();
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
            columnRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
