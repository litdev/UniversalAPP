using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniversalAPP.Tools;

namespace UniversalAPP.Web
{
    /// <summary>
    /// API接口身份验证
    /// </summary>
    public class APIAuthMiddleware
    {
        private RequestDelegate _next;
        private ILogger _logger;
        private Models.SiteConfig _config;

        public APIAuthMiddleware(RequestDelegate requestDelegate, ILoggerFactory factory, IOptionsSnapshot<Models.SiteConfig> appkeys)
        {
            _next = requestDelegate;
            _logger = factory.CreateLogger("Middleware");
            _config = appkeys.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            //不做安全验证
            if(!_config.WebAPIAuthentication)
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = 401;

            bool is_ok = context.Request.Headers.TryGetValue(_config.WebAPITokenKey, out Microsoft.Extensions.Primitives.StringValues monsterApiKeyHeaderValues);
            if (!is_ok)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(APIErrorMsg("缺少验证信息"));
                return;
            }
            string oauth = monsterApiKeyHeaderValues.First();
            if (string.IsNullOrWhiteSpace(oauth))
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(APIErrorMsg("TOKEN IS NULL"));
                return;
            }

            var descode_val = AESHelper.Decrypt(oauth, GlobalKeyConfig.AESKey, GlobalKeyConfig.AESIV);
            if (string.IsNullOrWhiteSpace(descode_val))
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(APIErrorMsg("TOKEN数据格式错误"));
                return;
            }

            string[] vals = descode_val.Split('!');
            if (vals.Length != 2)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(APIErrorMsg("TOKEN格式错误"));
                return;
            }
            if (!vals[0].Equals(_config.WebAPIMixer))
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(APIErrorMsg("TOKEN格式错误1"));
                return;
            }
            //超时时间为0表示只验证Token格式，不验证时间
            if (_config.WebAPITmeOutMinute <= 0)
            {
                context.Response.StatusCode = 200;
                await _next.Invoke(context);
            }
            else
            {
                DateTime dt_now = DateTime.Now;
                DateTime dt_old = WebHelper.ConvertToDateTime(TypeHelper.ObjectToLong(vals[1]));
                double diff = WebHelper.DateTimeDiff(dt_old, dt_now, "am");
                if (diff >= _config.WebAPITmeOutMinute)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(APIErrorMsg("TIME OUT"));
                    return;
                }
            }

            //验证成功后
            //var userNameClaim = new Claim(ClaimTypes.Name, "UserName");
            //var identity = new ClaimsIdentity(new[] { userNameClaim }, "MonsterAppApiKey");
            //context.User = new ClaimsPrincipal(identity);


            context.Response.StatusCode = 200;
            await _next.Invoke(context);

        }

        /// <summary>
        /// 接口返回错误验证信息
        /// </summary>
        /// <returns></returns>
        private string APIErrorMsg(string text)
        {
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>
            {
                msgbox = text
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }

    }

    public static class ContentExtensions
    {
        public static IApplicationBuilder UseAPIAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware(typeof(APIAuthMiddleware));
        }
    }

}
