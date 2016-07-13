using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PwCTools.Controllers
{
    public class BoardController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}