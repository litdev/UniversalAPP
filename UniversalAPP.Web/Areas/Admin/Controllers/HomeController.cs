using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace UniversalAPP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string s)
        {
            return View();
        }


        [AdminPermission("其他", "后台管理首页")]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Center()
        {
            return View();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginOut()
        {
            CustomAdminLoginUser.LoginOut(HttpContext);
            return Redirect("/Admin/Home/Login");
        }

        /// <summary>
        /// 登陆错误次数+1
        /// </summary>
        private void LoginFileTimesAdd()
        {
            //登陆次数+1
            if (HttpContext.Session.GetInt32(GlobalKeyConfig.Session_Login_Fail_Total)==null)
                HttpContext.Session.SetInt32(GlobalKeyConfig.Session_Login_Fail_Total, 1);
            else
                HttpContext.Session.SetInt32(GlobalKeyConfig.Session_Login_Fail_Total, Tools.TypeHelper.ObjectToInt(HttpContext.Session.GetInt32(GlobalKeyConfig.Session_Login_Fail_Total), 0) + 1);
        }

    }
}