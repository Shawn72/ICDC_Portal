using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using ICDC_Portal.Models;
using ICDC_Portal.OData;
using Login = ICDC_Portal.Models.Login;

namespace ICDC_Portal.Controllers
{
    public class LoginController : Controller
    {
        public NAV Nav = new NAV(new Uri(ConfigurationManager.AppSettings["ODATA_URI"]))
        {
            Credentials =
           new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
               ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"])
        };

        public static readonly string StrSqlConn = @"Server=" + ConfigurationManager.AppSettings["DB_INSTANCE"] + ";Database=" +
                                  ConfigurationManager.AppSettings["DB_NAME"] + "; User ID=" +
                                  ConfigurationManager.AppSettings["DB_USER"] + "; Password=" +
                                  ConfigurationManager.AppSettings["DB_PWD"] + "; MultipleActiveResultSets=true";

        public string CompanyName = "ICDC";

        // GET: Login
        public ActionResult SignIn()
        {
            return View("Login");
        }
        static string EncryptP(string mypass)
        {
            //encryptpassword:
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(mypass));
                return Convert.ToBase64String(data);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ICDC_Applicants_Register login)
        {
            var loginEntities = new ICDCEntities();
            var userName = loginEntities.ValidateClient(login.Username, EncryptP(login.Password)).FirstOrDefault();
           
            switch (userName)
            {
                case "invalid":
                    TempData["red"] = "You entered Invalid Account";
                    break;
                case "deactivated":
                    TempData["red"] = "You entered Inactive Account";
                    break;
                default:
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["username"] = login.Username;
                    return RedirectToAction("Index", "Home");

                //try
                //{
                //    using (SqlConnection con = new SqlConnection(StrSqlConn))
                //    {
                //        var myUserId = "";
                //        using (SqlCommand cmd = new SqlCommand("Validate_User"))
                //        {
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            cmd.Parameters.AddWithValue("@Username", login.Username);
                //            cmd.Parameters.AddWithValue("@Password", EncryptP(login.Password));
                //            cmd.Connection = con;

                //            con.Open();
                //            myUserId = Convert.ToString(cmd.ExecuteScalar());
                //            con.Close();

                //        }
                //        switch (myUserId)
                //        {
                //            case "invalid":
                //                TempData["red"] = "You entered Invalid Account";
                //                break;
                //            case "deactivated":
                //                TempData["red"] = "You entered Inactive Account";
                //                break;
                //            default:
                //                //  FormsAuthentication.RedirectFromLoginPage(userName, chkRememberMe.Checked);

                //                //FormsAuthentication.GetAuthCookie(login.Username, false);
                //                //FormsAuthentication.SetAuthCookie(login.Username, false);
                //                return RedirectToAction("Index");
                //        }
                //    }

                //}
                //catch (Exception ex)
                //{
                //    // ignored
                //}
            }
            return View(login);
        }
       

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Defaults");
        }

    }
   
}