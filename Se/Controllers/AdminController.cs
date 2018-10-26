using Se.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Controllers
{
    public class AdminController : Controller
    {
        private SeEntities db = new SeEntities();
        // GET: Admin
        [AdminRole]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult Login(string UserName,string Password)
        {
            var admin = db.Admins.FirstOrDefault(m => m.UserName == UserName && m.Password == Password);
            if (admin != null)
            {
                HttpContext.Response.AppendCookie(new HttpCookie("Auth", admin.Id.ToString()));
                return Json("true");
            }
            return Json("");
        }

    }
}