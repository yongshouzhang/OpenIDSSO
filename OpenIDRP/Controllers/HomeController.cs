using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Reflection;
using System.Web.Caching;
namespace OpenIDRP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // 声明 xrds 文档位置
            Response.AppendHeader(
            "X-XRDS-Location",
            new Uri(Request.Url, Response.ApplyAppPathModifier("~/Home/xrds")).AbsoluteUri);
            return View("Index");
        }

        public ActionResult Xrds()
        {
            return View();
        }

    }
}