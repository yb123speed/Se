using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Se.Models;
using QRCoder;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace Se.Controllers
{
    public class UserController : Controller
    {
        private SeEntities db = new SeEntities();

        [Authorize]
        // GET: Users
        public ActionResult Index()
        {
            var user = db.Users.Find(Convert.ToInt32(User.Identity.Name));
            if (user == null)
            {
                return HttpNotFound();
            }
            var dt = DateTime.Now.Date;
            var yesterday = dt.AddDays(-1);
            var day7 = dt.AddDays(-1);


            var yesterdayBonus = db.BonusLogs.Where(m => m.ToUserId == user.Id && (m.BonusTime > yesterday && m.BonusTime < dt));
            ViewBag.Yesterday = yesterdayBonus.Count()>0?yesterdayBonus.Sum(m => m.Cash):(decimal)0;
            var day7Bonus = db.BonusLogs.Where(m => m.ToUserId == user.Id && (m.BonusTime > yesterday && m.BonusTime < dt));
            ViewBag.Day7ShouYi = day7Bonus.Count()>0? day7Bonus.Sum(m=>m.Cash):(decimal)0;
            var allBonus = db.BonusLogs.Where(m => m.ToUserId == user.Id);
            ViewBag.AllBonus = allBonus.Count()>0?allBonus.Sum(m=>m.Cash):(decimal)0;
            var tixianBonus = db.Withdraws.Where(m => m.UserId == user.Id && m.Status == (int)WithdrawStatus.提现中);
            ViewBag.TiXianZhong = tixianBonus.Count()>0?tixianBonus.Sum(m => m.Cash):(decimal)0;
            return View(user);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        #region login

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string UserName, string Password)
        {
            var user = db.Users.FirstOrDefault(m=>m.UserName==UserName&&m.Password==Password);
            if (user != null)
            {
                if (user.Status == 1)
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                    return Json("true");
                }
                else
                {
                    return Json("status");
                }
            }
            return Json("false");
        }


        #endregion

        #region register
        public ActionResult Register(int Id = 0)
        {
            ViewBag.InviteCode = Id;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Register([Bind(Include = "UserName,Password,NickName,ParentId")] User user, string ConfirmPassword, string ValiteCode)
        {
            try
            {
                user.NickName = user.UserName;
                user.RegisterTime = DateTime.Now;
                user.Bonus = 0;
                user.Withdraw = 0;
                user.Status = 1;

                #region repwd
                if (ConfirmPassword != user.Password)
                {
                    return Json("repwd");
                }

                #endregion

                #region ValideCode
                string _valideCode = Session["CheckCode"].ToString();
                if (_valideCode != ValiteCode.ToLower())
                {
                    return Json("false");
                }
                #endregion

                #region Parent

                if (user.ParentId > 0 && db.Users.FirstOrDefault(m => m.Id == user.ParentId) == null)
                {
                    return Json("noaff");
                }

                #endregion

                #region UserName

                if (db.Users.FirstOrDefault(m => m.UserName == user.UserName) != null)
                {
                    return Json("exist");
                }


                #endregion


                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Json("true");
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        #endregion

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,Password,NickName,RegisterTime,ParentId,Bonus,Withdraw,Status")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ValidateCode()
        {
            string CCode = Se.Models.ValidateCode.CreateValidateCode(5);
            Session["CheckCode"] = CCode.ToLower();
            return File(Se.Models.ValidateCode.CreateValidateGraphic(CCode), @"image/jpeg");
        }

        [Authorize]
        public ActionResult InviteCodeShow()
        {   
            return View();
        }
        

        [Authorize]
        public ActionResult InviteQRCode()
        {
            string url = Request.Url.Scheme + "://" + Request.Url.Host+"/User/Register/"+ User.Identity.Name;
            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”
            Bitmap qrCodeImage = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            return File(ms.ToArray(), @"image/jpeg");
        }

        [Authorize]
        public ActionResult AffShow()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAffUserList(int affid)
        {
            var list = db.Users.Where(m => m.ParentId == affid).ToList();
            return Json(list);
        }

        public int GetUserAffUserCount(int affid)
        {
            var count = db.Users.Where(m => m.ParentId == affid).Count();
            return count;
        }

        [Authorize]
        public ActionResult BonusList()
        {
            int uid = Convert.ToInt32(User.Identity.Name);
            var bonusLogList = db.BonusLogs.Where(m => m.ToUserId == uid);
            return View(bonusLogList);
        }

        [Authorize]
        public ActionResult Mine()
        {
            return View();
        }
    }
}
