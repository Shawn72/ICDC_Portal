using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using ICDC_Portal.Models;
using ICDC_Portal.OData;
using ICDC_Portal.WSRAPI;

namespace ICDC_Portal.Controllers
{
   
    public class UserRegistrationController : Controller
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
        // GET: User_Registration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveUserAccountDetails(){

            return View();
        }
        [HttpPost]
        //public JsonResult SaveUserAccountDetails(DetailsInsertClass regUser)
        //{
        //    bool status = InsertData(regUser);

        //    return new JsonResult { Data = status };
        //}

        public ActionResult SaveUserAccountDetails(DetailsInsertClass regUser)
        {
            bool status = InsertData(regUser);
            switch (status)
            {
                case true:
                    TempData["Success"] = "Your Account Successfully created, Go to your EMAIL to activate";
                    ViewBag.Message = "Your Account Successfully created, Go to your EMAIL to activate";
                    break;
                case false:
                    TempData["Failed"] = "Failed to create Account, try again later!";
                    ViewBag.Message= "Failed to create Account, try again later!";
                    break;
            }
           // return View("SaveUserAccountDetails");
            return RedirectToAction("SaveUserAccountDetails");
        }

        protected bool InsertData(DetailsInsertClass regUser)
        {
            bool datasaved = false;
            try
            {
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"], ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);
                WebPortal webp = new WebPortal();
                webp.Credentials = credentials;
                webp.PreAuthenticate = true;
                if (webp.FnCreateApplicants(regUser.FirstName, regUser.MiddleName, regUser.LastName, regUser.UserName,regUser.Email, EncryptP(regUser.Password2), regUser.ActivationCode.ToString()))
                {
                    //  this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "getmeTohomeConfirm()", true);

                    //call goback to login to load Default page for user

                    InsertToActivationDb(regUser.UserName, regUser.ActivationCode.ToString());

                    using (MailMessage mm = new MailMessage("ngutumbuvi8@gmail.com", regUser.Email))
                    {
                        
                        mm.Subject = "ICDC Account Activation";
                        string body = "Hello " + regUser.UserName + ",";
                        body += "<br /><br />Please click the following link to activate your account";
                        if (Request.Url != null)
                            body += "<br /><a href = '" + string.Format("{0}://{1}/UserRegistration/ActivateMyAccount/{2}", Request.Url.Scheme, Request.Url.Authority, regUser.ActivationCode) + "'>Click here to activate your account.</a>";
                        body += "<br /><br />Thanks";
                        mm.Body = body;
                        mm.IsBodyHtml = true;

                        //when no proxy used
                        SmtpClient smtp = new SmtpClient();


                        //string smtpAddress = "smtp.mail.yahoo.com";
                        //int portNumber = 587;
                        //bool enableSSL = true;



                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential networkCred = new NetworkCredential("ngutumbuvi8@gmail.com", "ngutu12345");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = networkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                        
                    }
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
        }
            catch (Exception ex)
            {
                // ignored
            }
            return datasaved;
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

        public ActionResult EditProfile()
        {
            return View();
        }

        public ActionResult ActivateMyAccount()
        {
            try
            {
                
                if (RouteData.Values["id"] != null)
                {
                    Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                    ActivatedfromDb(activationCode);
                }
                
            }
            catch (Exception ex)
            {
                // ignored
            }
            return View();
        }

        public void ActivatedfromDb(Guid actiVCode)
        {
            var credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"], ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);
            WebPortal webp = new WebPortal();
            webp.Credentials = credentials;
            webp.PreAuthenticate = true;

            var activatem = Nav.applicantsRegister.ToList().Where(r => r.Activation_Code == actiVCode.ToString());
            string activatemyAss = activatem.Select(r => r.Activation_Code).SingleOrDefault();
            using (SqlConnection con = new SqlConnection(StrSqlConn))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM UserActivation WHERE ActivationCode = @ActivationCode"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ActivationCode", actiVCode);
                        cmd.Connection = con;
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();
                        if (rowsAffected == 1)
                        {
                            bool status = webp.FnActivateAc(activatemyAss);
                            switch (status)
                            {
                                case true:
                                    //successful activation
                                    TempData["green"] = "Your Account Activated Successfully";
                                    break;
                                //failed activation
                                case false:
                                    TempData["red"] = "Failed to activate account";
                                    break;
                            }
                           
                        }
                        else
                        {
                            //failed activation
                            TempData["red"] = "Account Already active!";
                        }
                    }
                }
            }
        }
        public void InsertToActivationDb(string userName, string actvCode)
        {
            using (SqlConnection con = new SqlConnection(StrSqlConn))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO UserActivation VALUES(@UserId, @ActivationCode)"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserId", userName);
                        cmd.Parameters.AddWithValue("@ActivationCode", actvCode);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
    }
}