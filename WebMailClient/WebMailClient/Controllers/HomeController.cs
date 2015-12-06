using IMAP.Base;
using IMAP.Base.Framework;
using IMAP.Base.Imap;
using IMAP.Base.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMailClient.Models;
using WebMailClient.viewmodel;

namespace WebMailClient.Controllers
{
    public class HomeController : Controller
    {
        private ImapBase context;
        public HomeController()
        {

        }
        public ActionResult Index()
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var imap = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msgs = imap.SearchMessages(
                      SearchCondition.Undeleted().And(
                        SearchCondition.SentSince(new DateTime(2000, 1, 1))
                      )
                    );
                    var msges = imap.GetMessages(0, msgs.Count());
                    inboxviewmodel vmodel = new inboxviewmodel();
                    vmodel.AllMessages = new List<MailMessage>();
                    foreach (MailMessage item in msges.OrderByDescending(s=>s.Date))
                    {
                        vmodel.AllMessages.Add(item);
                    }
                    return View(vmodel);
                }
            }
        }

        public ActionResult Detail(int id)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var context = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    MailMessage msg = context.GetMessage(id.ToString());
                    Session["MSG"] = msg;
                    return View(msg);
                }
            }
        }

        public FileResult Download(attachementviewmodel attachment)
        {
            string fileName = attachment.FileName; //attachment.Filename;
            MailMessage msg = (MailMessage)Session["MSG"];
            byte[] fileBytes = msg.Attachments.Where(s => s.Filename == fileName).FirstOrDefault().GetData();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }
        public ActionResult Draft()
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var imap = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msgs = imap.SearchMessages(
                      SearchCondition.Draft()
                    );

                    var msges = imap.GetMessages(0, msgs.Count());
                    inboxviewmodel vmodel = new inboxviewmodel();
                    vmodel.AllMessages = new List<MailMessage>();
                    foreach (MailMessage item in msges.OrderByDescending(s => s.Date))
                    {
                        vmodel.AllMessages.Add(item);
                    }
                    return View(vmodel);
                }
            }
            return View();
        }
        public ActionResult DraftOpen(int id)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Forward";
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var context = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msg = context.GetMessage(id.ToString());
                    mailmessageviewmodel msgviewmodel = new mailmessageviewmodel();
                    msgviewmodel.Subject = msg.To.FirstOrDefault().ToString();
                    msgviewmodel.Body = msg.Body;
                    return View(msgviewmodel);
                }
            }
        }

        public ActionResult New()
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Compose New Email";
                mailmessageviewmodel model = new mailmessageviewmodel();
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult New(mailmessageviewmodel ModelMessage)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                try
                {
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("smtp.gmail.com");

                    mail.From = new System.Net.Mail.MailAddress(model.Email);
                    mail.To.Add(ModelMessage.To);
                    mail.Subject = ModelMessage.Subject;
                    mail.Body = ModelMessage.Body;

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(model.Email, model.Password);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    ViewBag.Alert = "Email Send Successfully";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Alert = "Error while sending mail";
                    return View();
                }
            }
        }

        public ActionResult Reply(messageinfoviewmodel param)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Reply";
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var context = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msg = context.GetMessage(param.uid.ToString());
                    mailmessageviewmodel msgviewmodel = new mailmessageviewmodel();
                    msgviewmodel.To = msg.From.ToString();
                    msgviewmodel.Subject = "Re: " + msg.To.FirstOrDefault().ToString();
                    msgviewmodel.Body = msg.Body + ">>>Reply==================================================";
                    return View(msgviewmodel);
                }
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Reply(mailmessageviewmodel ModelMessage)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                try
                {
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("smtp.gmail.com");

                    mail.From = new System.Net.Mail.MailAddress(model.Email);
                    if (ModelMessage.To.Contains("<"))
                    {
                        string actualEmail = ModelMessage.To.Split('<', '>')[1];
                        mail.To.Add(actualEmail);
                    }
                    else
                    {
                        mail.To.Add(ModelMessage.To);
                    }
                    mail.Subject = ModelMessage.Subject;
                    mail.Body = ModelMessage.Body;

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(model.Email, model.Password);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    ViewBag.Alert = "Email Send Successfully";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Alert = "Error while sending mail";
                    return View();
                }
            }
        }

        public ActionResult Forward(messageinfoviewmodel param)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Forward";
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var context = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msg = context.GetMessage(param.uid.ToString());
                    mailmessageviewmodel msgviewmodel = new mailmessageviewmodel();
                    msgviewmodel.Subject = "Re: " + msg.To.FirstOrDefault().ToString();
                    msgviewmodel.Body = msg.Body + ">>>Forward==================================================";
                    return View(msgviewmodel);
                }
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Forward(mailmessageviewmodel ModelMessage)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoginViewModel model = (LoginViewModel)Session["Model"];
                try
                {
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("smtp.gmail.com");

                    mail.From = new System.Net.Mail.MailAddress(model.Email);
                    if (ModelMessage.To.Contains("<"))
                    {
                        string actualEmail = ModelMessage.To.Split('<', '>')[1];
                        mail.To.Add(actualEmail);
                    }
                    else
                    {
                        mail.To.Add(ModelMessage.To);
                    }
                    mail.Subject = ModelMessage.Subject;
                    mail.Body = ModelMessage.Body;

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(model.Email, model.Password);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    ViewBag.Alert = "Email Send Successfully";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Alert = "Error while sending mail";
                    return View();
                }
            }
        }

        public ActionResult Delete(messageinfoviewmodel param)
        {
            if (Session["Model"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Forward";
                LoginViewModel model = (LoginViewModel)Session["Model"];
                bool isSSL = true;
                using (var context = new ImapBase("imap.gmail.com", model.Email, model.Password, AuthMethods.Login, 993, isSSL))
                {
                    var msg = context.GetMessage(param.uid.ToString());
                    context.DeleteMessage(msg);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        private void SendEmail(object sender, IMAP.Base.Imap.MessageEventArgs e)
        {
            Mailbox box = new Mailbox("");
        }

    }
}