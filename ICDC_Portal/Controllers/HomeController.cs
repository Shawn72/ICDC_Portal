using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ICDC_Portal.OData;

namespace ICDC_Portal.Controllers
{
    public class HomeController : Controller
    {
        public NAV Nav = new NAV(new Uri(ConfigurationManager.AppSettings["ODATA_URI"]))
        {
            Credentials =
         new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
             ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"])
        };
        public ActionResult Index()
        {
           return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateResume()
        {
            var listform = Nav.applicantsRegister.Where(r => r.Username == Session["username"].ToString()).ToList();

            return View(listform);
        }
    }
}