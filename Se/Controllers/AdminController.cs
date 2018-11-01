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

        #region 会员
        public ActionResult MemberList(int p = 1, int ps = 20, string kw="")
        {
            ViewBag.P=p;
            ViewBag.KW=kw;
            if (p <= 0)
            {
                p = 1;
            }
            if (ps <= 0)
            {
                ps = 20;
            }
            var members = db.Users.Where(m => true);
            if (string.IsNullOrWhiteSpace(kw))
            {

            }
            else
            {
                members = members.Where(m => m.UserName.Contains(kw));
            }
            var count = members.Count();
            ViewBag.Count = count;
            if (count % ps > 0)
            {
                ViewBag.PC = count / ps + 1;
            }
            else
            {
                ViewBag.PC = count / ps;
            }
            members = members.OrderByDescending(m => m.Id).Skip((p - 1) * ps).Take(ps);

            return View(members);
        }

        [HttpPost]
        public ActionResult MemberChangeStatus(int id, int status)
        {
            if (status > 1 || id < 1)
            {
                return HttpNotFound();
            }
            var u = db.Users.Find(id);
            u.Status = status;
            db.SaveChanges();
            return Json(true);
        }
        #endregion

        #region 下单

        public ActionResult OrderList(int p = 1, int ps = 20, string kw = "")
        {
            ViewBag.P = p;
            ViewBag.KW = kw;
            if (p <= 0)
            {
                p = 1;
            }
            if (ps <= 0)
            {
                ps = 20;
            }
            var orders = db.Orders.Where(m => true);
            if (string.IsNullOrWhiteSpace(kw))
            {

            }
            else
            {
                orders = orders.Where(m => m.UserName.Contains(kw));
            }
            var count = orders.Count();
            ViewBag.Count = count;
            if (count % ps > 0)
            {
                ViewBag.PC = count / ps + 1;
            }
            else
            {
                ViewBag.PC = count / ps;
            }
            orders = orders.OrderByDescending(m => m.Id).Skip((p - 1) * ps).Take(ps);
            var result =orders.ToList();

            return View(result);
        }

        public ActionResult Order()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Order([Bind(Include = "Title,Cash,UserName,UserId,Remark")] Order order)
        {
            order.OrderTime = DateTime.Now;
            var u = db.Users.FirstOrDefault(m => m.UserName == order.UserName);
            if (u == null)
            {
                return View();
            }
            order.UserId = u.Id;
            db.Orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        #endregion

    }
}