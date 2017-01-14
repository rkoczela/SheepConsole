using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSheepshead.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SheepGame()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Sheepshead AHHHHHHH.";

            return View();
        }
    }
}