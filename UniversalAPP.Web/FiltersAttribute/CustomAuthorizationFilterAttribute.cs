using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using UniversalAPP.Tools;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 身份验证过滤器
    /// </summary>
    public class CustomAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorizationFilterAttribute> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private EFCore.EFDBContext _db_context;
        private readonly Models.SiteBasicConfig _config;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomAuthorizationFilterAttribute(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider, ILogger<CustomAuthorizationFilterAttribute> logger, EFCore.EFDBContext db, IOptionsSnapshot<Models.SiteBasicConfig> siteConfig)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _config = siteConfig.Value;
            _db_context = db;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                //如果是开发环境，则跳过验证
                if (_hostingEnvironment.IsDevelopment()) return;

                //APIAuth(context);
                return;
            }
            else if (context.HttpContext.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
            {
                ////添加了AllowAnonymous属性则要跳过验证
                //if (context.Filters.Any(p => p is IAllowAnonymousFilter)) return;
                ////判断用户是否登录


                //后台管理的权限验证
                //获取控制器上的
                var check_power = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes.Any(p => p.AttributeType == typeof(AdminPermissionAttribute));
                if (!check_power) return;
                //验证
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                string route = controller + "/" + action;
                var IsPost = context.HttpContext.Request.Method == "POST";

                //int login_user_id = TypeHelper.ObjectToInt(context.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
                //int login_user_role_id = TypeHelper.ObjectToInt(context.HttpContext.User.FindFirst(GlobalKeyConfig.Admin_Claim_RoleID).Value);
                bool login_user_role_super = TypeHelper.ObjectToBool(context.HttpContext.User.FindFirst(GlobalKeyConfig.Admin_Claim_IsSuperAdminRole).Value);
                if (!login_user_role_super && !context.HttpContext.User.HasClaim(route, IsPost.ToString()))
                {
                    if (IsPost)
                    {
                        ResultAPIAuthMsg(context, "没有权限");
                    }
                    else
                    {
                        var result_view = new ViewResult { StatusCode = 403, ViewName = "Prompt" };
                        result_view.ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                        result_view.ViewData.Model = new PromptModel("没有权限", "请联系管理员以获取访问权限");
                        context.Result = result_view;
                    }
                }

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
