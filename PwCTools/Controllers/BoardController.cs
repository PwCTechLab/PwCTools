using PwCTools.Models;
using PwCTools.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PwCTools.Controllers
{
    public class BoardController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            using (var repo = new ProjectRepository(new ApplicationDbContext()))
            {
                string id = User.Identity.GetUserId();

                var projects = repo.GetUserProjects(id);
                int? defaultProject = repo.GetDefaultProject(id);

                if (defaultProject == null && projects.Count > 0)
                    defaultProject = projects.FirstOrDefault().Id;

                ViewBag.ProjectList = new SelectList(projects, "Id", "Name", defaultProject);
            }
                
            return View();
        }
    }
}