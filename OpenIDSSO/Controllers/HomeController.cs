using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIDSSO.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Request.AcceptTypes.Contains("application/xrds+xml"))
            {
                ViewData["OPIdentifier"] = true;
                return View("Xrds");
            }
            return View();
        }
    }
}