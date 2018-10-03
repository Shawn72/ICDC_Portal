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
    public class UserAccountDetailsController : Controller
    {
        public NAV Nav = new NAV(new Uri(ConfigurationManager.AppSettings["ODATA_URI"]))
        {
            Credentials =
          new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
              ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"])
        };
        // GET: UserAccountDetails
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUserDetails()
        {
            var listform = Nav.applicantsRegister.Where(r => r.Username == Session["username"].ToString()).ToList();

            return View(listform);
        }
    }
}