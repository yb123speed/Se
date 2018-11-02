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
            ViewBag.UserDict = db.Users.Where(m => m.Status == 1).Select<User,UserDictVM>(m => new UserDictVM
            {
                UserId=m.Id,
                UserName=m.UserName
            });
            return View();
        }

        [HttpPost]
        public ActionResult Order([Bind(Include = "Title,Cash,UserId,Remark")] Order order)
        {
            order.OrderTime = DateTime.Now;

            var u = db.Users.AsNoTracking().FirstOrDefault(m => m.Id == order.UserId);
            if (u == null)
            {
                return View();
            }
            order.UserName = u.UserName;
            using (var context = new SeEntities())
            using (var trans = context.Database.BeginTransaction())
            {
                #region 第一代
                if (u.ParentId > 0)//有上级
                {
                    User u1 = context.Users.FirstOrDefault(m => m.Id == u.ParentId);
                    if (u1 != null)
                    {
                        BonusLog bonusLog1 = new BonusLog
                        {
                            UserId = u.Id,
                            ToUserId = u1.Id,
                            BonusTime = DateTime.Now,
                            BonusType = (int)BonusType.TuiJian,
                            Cash = 900,
                            OldCash = u1.Bonus,
                            NewCash = u1.Bonus + 900
                        };
                        u1.Bonus = bonusLog1.NewCash;
                        context.BonusLogs.Add(bonusLog1);

                        #region 第二代
                        if (u1.ParentId > 0)
                        {
                            User u2 = context.Users.FirstOrDefault(m => m.Id == u1.ParentId);
                            if (u2 != null)
                            {
                                BonusLog bonusLog2 = new BonusLog
                                {
                                    UserId = u1.Id,
                                    ToUserId = u2.Id,
                                    BonusTime = DateTime.Now,
                                    BonusType = (int)BonusType.TuiJian,
                                    Cash = 700,
                                    OldCash = u2.Bonus,
                                    NewCash = u2.Bonus + 700
                                };
                                u2.Bonus = bonusLog2.NewCash;
                                context.BonusLogs.Add(bonusLog2);

                                #region 第三代
                                if (u2.ParentId > 0)
                                {
                                    User u3 = context.Users.FirstOrDefault(m => m.Id == u2.ParentId);
                                    if (u3 != null)
                                    {
                                        BonusLog bonusLog3 = new BonusLog
                                        {
                                            UserId = u2.Id,
                                            ToUserId = u3.Id,
                                            BonusTime = DateTime.Now,
                                            BonusType = (int)BonusType.TuiJian,
                                            Cash = 700,
                                            OldCash = u3.Bonus,
                                            NewCash = u3.Bonus + 700
                                        };
                                        u3.Bonus = bonusLog3.NewCash;
                                        context.BonusLogs.Add(bonusLog3);
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                    }
                }
                #endregion

                context.Orders.Add(order);
                try
                {
                    context.SaveChanges();
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
            return RedirectToAction("OrderList");
        }

        #endregion

    }
}