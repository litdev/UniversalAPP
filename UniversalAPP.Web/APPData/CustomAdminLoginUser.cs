using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 后台登录用户操作
    /// </summary>
    public class CustomAdminLoginUser
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="context">请求上下文</param>
        public static void Login(HttpContext context)
        {
            
        }

        ///// <summary>
        ///// 用户登出
        ///// </summary>
        ///// <param name="context"></param>
        //public Task LoginOut(HttpContext context)
        //{            
        //    return Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //}
               


    }
}
