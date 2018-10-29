using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Se.Models
{
    public class AdminRole : AuthorizeAttribute, IAuthorizationFilter
    {
        private SeEntities db = new SeEntities();
        public string RoleString;

        public AdminRole()
        {
            this.RoleString = "";
        }

        public AdminRole(string rolestring)
        {
            this.RoleString = rolestring;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContextBase b = filterContext.RequestContext.HttpContext;
            var adminIdStr = b.Request.Cookies["Auth"];

            var redirectUrl = "/Admin/Login";
            if (adminIdStr==null||Convert.ToInt32(adminIdStr.Value)<=0)
            {
                filterContext.Result = new RedirectResult(redirectUrl);
                return;
            }
            //int id = Convert.ToInt32(adminIdStr);
            //Admin u = db.Admins.FirstOrDefault(m=>m.Id==id);
            //if (u != null)
            //{
            //    if (RoleString != null && (u.Root != null && u.Root.Split(',').Count() >= 4) && u.Status == 1)
            //    {
            //        int role = RoleType(RoleString);
            //        string[] roots = u.Root.Split(',');
            //        if (roots[role] != "1")
            //        {
            //            filterContext.HttpContext.Response.Write("<script type='text/javascript'>alert('暂无权限，请联系管理员确认你是否由此管理权限');</script>");
            //            filterContext.HttpContext.Response.End();

            //            return;
            //        }
            //    }
            //    else
            //    {
            //        filterContext.HttpContext.Response.Write("<script type='text/javascript'>alert('暂无权限，请联系管理员确认你是否由此管理权限');</script>");
            //        filterContext.HttpContext.Response.End();

            //        return;
            //    }
            //}
            //else
            //{
            //    filterContext.Result = new RedirectResult(redirectUrl);
            //    return;
            //}
        }

        public int RoleType(string rolestring)
        {
            int role = 0;
            switch (rolestring)
            {
                case "User":
                    role = 0;
                    break;
                case "Meal":
                    role = 1;
                    break;
                case "Article":
                    role = 2;
                    break;
                case "GBook":
                    role = 3;
                    break;
                case "Cash":
                    role = 4;
                    break;
                case "Admin":
                    role = 5;
                    break;
                case "System":
                    role = 6;
                    break;
                default:
                    role = 0;
                    break;
            }

            return role;
        }
    }
}