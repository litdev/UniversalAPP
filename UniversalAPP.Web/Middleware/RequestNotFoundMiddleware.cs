using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniversalAPP.Tools;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 请求出错处理
    /// </summary>
    public class RequestNotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestNotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            //API
            if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                if(context.Response.StatusCode == 404)
                {
                    UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
                    //result.msgbox = "未找到接口，请确定URL地址是否有误";
                    result.msgbox = $"No Address Found";
                    result.ext_str = $"Request URL:{context.Request.Path.Value}";
                    await context.Response.WriteAsync(JsonHelper.ToJson(result),System.Text.Encoding.UTF8);
                }
            }
        }

    }

    public static class RequestNotFoundMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestNotFound(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestNotFoundMiddleware>();
        }
    }

}
