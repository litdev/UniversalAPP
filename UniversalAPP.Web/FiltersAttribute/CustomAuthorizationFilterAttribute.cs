using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using UniversalAPP.Tools;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 身份验证过滤器
    /// </summary>
    public class CustomAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorizationFilterAttribute> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly Models.SiteConfig _config;

        public CustomAuthorizationFilterAttribute(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory, IOptionsSnapshot<Models.SiteConfig> siteConfig)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<CustomAuthorizationFilterAttribute>();
            _config = siteConfig.Value;

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                //如果是开发环境，则跳过验证
                if (_hostingEnvironment.IsDevelopment()) return;

                APIAuth(context);
                return;
            }
            else if (context.HttpContext.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
            {
                //添加了AllowAnonymous属性则要跳过验证
                if (context.Filters.Any(p => p is IAllowAnonymousFilter)) return;
                //判断用户是否登录


                //后台管理的身份验证
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                string route = controller + "/" + action;



            }
            else
            {
                //前台页面的验证
            }


        }

        #region  接口验证方法

        /// <summary>
        /// 接口验证
        /// </summary>
        /// <param name="context"></param>
        private void APIAuth(AuthorizationFilterContext context)
        {
            //不做安全验证
            if (!_config.WebAPIAuthentication) return;

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = 401;

            Microsoft.Extensions.Primitives.StringValues monsterApiKeyHeaderValues;
            bool is_ok = context.HttpContext.Request.Headers.TryGetValue(_config.WebAPITokenKey, out monsterApiKeyHeaderValues);
            if (!is_ok)
            {
                ResultAPIAuthMsg(context, "缺少验证信息");
                return;
            }
            string oauth = monsterApiKeyHeaderValues.First();
            if (string.IsNullOrWhiteSpace(oauth))
            {
                ResultAPIAuthMsg(context, "TOKEN IS NULL");
                return;
            }

            var descode_val = AESHelper.Decrypt(oauth, GlobalKeyConfig.AESKey, GlobalKeyConfig.AESIV);
            if (string.IsNullOrWhiteSpace(descode_val))
            {
                ResultAPIAuthMsg(context, "TOKEN数据格式错误");
                return;
            }

            string[] vals = descode_val.Split('!');
            if (vals.Length != 2)
            {
                ResultAPIAuthMsg(context, "TOKEN格式错误");
                return;
            }
            if (!vals[0].Equals(_config.WebAPIMixer))
            {
                ResultAPIAuthMsg(context, "TOKEN格式错误1");
                return;
            }

            //超时时间为0表示只验证Token格式，不验证时间
            if (_config.WebAPITmeOutMinute <= 0) return;
            else
            {
                DateTime dt_now = DateTime.Now;
                DateTime dt_old = WebHelper.ConvertToDateTime(TypeHelper.ObjectToLong(vals[1]));
                double diff = WebHelper.DateTimeDiff(dt_old, dt_now, "am");
                if (diff >= _config.WebAPITmeOutMinute)
                {

                    ResultAPIAuthMsg(context, "TIME OUT");
                    return;
                }
            }

            context.HttpContext.Response.StatusCode = 200;
            return;
        }

        /// <summary>
        /// 接口返回错误验证信息
        /// </summary>
        /// <returns></returns>
        private void ResultAPIAuthMsg(AuthorizationFilterContext context, string text)
        {
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
            result.msgbox = text;
            context.Result = new ObjectResult(result);
        }


        #endregion

    }
}
