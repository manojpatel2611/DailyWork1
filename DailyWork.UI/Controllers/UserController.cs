using DailyWork.Data.Models;
using DailyWork.UI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DailyWork.UI.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Save()
        {
            UserMaster userMaster = new UserMaster();
            if (userMaster.UserID == 0)
            {
                #region activation code
                userMaster.ActivationCode = Guid.NewGuid();
                #endregion

                #region password hashing
                userMaster.Password = Crypto.Hash(Request.Form["Password"]);
                #endregion

                #region Email is verified
                userMaster.IsEmailVerfied = false;
                userMaster.IsSubscribedNewsLetter = false;
                #endregion

                using (RojMelEntities db = new RojMelEntities())
                {
                    #region check email is exist or not
                    var isExist = isEmailExist(Request.Form["Email"]);
                    if (isExist)
                    {
                        #region sav user to database
                        userMaster.DateOfBirth = Convert.ToDateTime(Request.Form["DateOfBirth"]);
                        userMaster.Email = Request.Form["Email"];
                        userMaster.FirstName = Request.Form["FirstName"];
                        userMaster.LastName = Request.Form["LastName"];
                        userMaster.Gender = Request.Form["Gender"];
                        userMaster.Mobile = Convert.ToInt32(Request.Form["Mobile"]);
                        userMaster.Password = Request.Form["Password"];
                        userMaster.CreatedBy = 1;
                        userMaster.CreatedOn = DateTime.Now;
                        userMaster.UpdateBy = null;
                        userMaster.UpdateOn = null;
                        db.UserMasters.Add(userMaster);
                        db.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Email is already esixt" });
                    }
                    #endregion
                }
                SendVarificationMail(userMaster.Email, userMaster.ActivationCode.ToString());
                return Json(new { Success = false, Message = "User is added. Actication code is sent to your email address" });
            }
            else
            {
                RojMelEntities db = new RojMelEntities();
                var userEdit = db.UserMasters.Where(x => x.UserID == userMaster.UserID).FirstOrDefault();
                userEdit.DateOfBirth = Convert.ToDateTime(Request.Form["DateOfBirth"]);
                userEdit.Email = Request.Form["Email"];
                userEdit.FirstName = Request.Form["FirstName"];
                userEdit.LastName = Request.Form["LastName"];
                userEdit.Gender = Request.Form["Gender"];
                userEdit.Mobile = Convert.ToInt32(Request.Form["Mobile"]);
                userEdit.Password = Request.Form["Password"];
                userEdit.UpdateBy = 1;
                userEdit.UpdateOn = DateTime.Now;
                db.SaveChanges();
                return Json(new { Success = true, Message = "User data is updated" });
            }
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            using (RojMelEntities ds = new RojMelEntities())
            {
                ds.Configuration.ValidateOnSaveEnabled = false;
                var v = ds.UserMasters.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerfied = true;
                    ds.SaveChanges();

                }
                else
                {
                    return Json(new { Success = false, Message = "Please try again " });
                }
            }
            return Json(new { Success = true, Message = "Your mail address is verified" });
        }

        [NonAction]
        public Boolean isEmailExist(string emialid)
        {
            using (RojMelEntities ds = new RojMelEntities())
            {
                var v = ds.UserMasters.Where(a => a.Email == emialid).FirstOrDefault();
                return v == null;
            }
        }
        [NonAction]
        public void SendVarificationMail(string email, string activationCode)
        {
            var varifyUrl = $"/User/verifyAccount/{activationCode}";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, varifyUrl);
            var fromEmail = new MailAddress("mcauvpcegnu@gmail.com", "MCA Parivar");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "mca12345678";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryFormat = SmtpDeliveryFormat.International,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Your account is successfully created!",
                Body = "<br/><br/>We are exited told you that your account is " + "Successfully created. Please click on below link to activate your account" +
                "<br/><br/><a href='" + link + "'>" + link + "</a>",
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}