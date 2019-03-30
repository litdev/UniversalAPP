using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;

namespace UniversalAPP.Web
{
    public class BaseAdminController : Controller
    {
        protected IHostingEnvironment _env;
        protected EFCore.EFDBContext _db_context;

        public BaseAdminController()
        {
            var hca = ServiceLocator.Instance.GetService<IHttpContextAccessor>();
            _env = hca.HttpContext.RequestServices.GetService<IHostingEnvironment>();
            _db_context = hca.HttpContext.RequestServices.GetService<EFCore.EFDBContext>();

        }

        /// <summary>
        /// 基础API返回String类型
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgbox"></param>
        /// <param name="data"></param>
        /// <param name="ext_int"></param>
        /// <param name="ext_str"></param>
        /// <returns></returns>
        protected JsonResult ResultBasicString(int msg, string msgbox, string data, int ext_int = 0, string ext_str = "")
        {
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
            result.msg = msg;
            result.msgbox = msgbox;
            result.data = data;
            result.ext_int = ext_int;
            result.ext_str = ext_str;
            return Json(result);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="LogMethodInDB">配置文件</param>
        /// <param name="LogType"></param>
        /// <param name="detail"></param>
        /// <param name="user_id"></param>
        protected void AddMethodLog(bool LogMethodInDB, Entity.SysLogMethodType LogType, string detail, int user_id = 0)
        {
            if (!LogMethodInDB) return;
            if (user_id == 0) user_id = Tools.TypeHelper.ObjectToInt(HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
            var entity = new Entity.SysLogMethod()
            {
                AddTime = DateTime.Now,
                Detail = detail,
                SysUserID = user_id,
                Type = LogType
            };
            new BLL.DynamicBLL<Entity.SysLogMethod>(_db_context).Add(entity);
        }

        #region 提示信息视图
        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="details">详细消息</param>
        /// <param name="message">简略消息</param>
        /// <returns></returns>
        protected ViewResult PromptView(string message, string details)
        {
            return View("Prompt", new PromptModel(message, details));
        }

        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        protected ViewResult PromptView(PromptModel model)
        {
            return View("Prompt", model);
        }

        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="linkUrl">跳转地址</param>
        /// <param name="message">简略消息</param>
        /// <param name="details">详细消息</param>
        /// <returns></returns>
        protected ViewResult PromptView(string linkUrl, string message, string details)
        {
            return View("Prompt", new PromptModel(linkUrl, message, details));
        }

        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="linkUrl">跳转地址</param>
        /// <param name="message">简略消息</param>
        /// <param name="details">详细消息</param>
        /// <param name="time">倒计时</param>
        /// <returns></returns>
        protected ViewResult PromptView(string linkUrl, string message, string details, int time)
        {
            return View("Prompt", new PromptModel(linkUrl, message, details, time));
        }
        #endregion

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="page_key"></param>
        /// <param name="is_post"></param>
        /// <returns></returns>
        protected bool CheckAdminPower(string page_key, bool is_post)
        {
            if (string.IsNullOrWhiteSpace(page_key))
            {
                page_key = GetControllerRoutePath();
                is_post = HttpContext.Request.Method == "POST";
            }
            page_key = page_key.ToLower();
            //int login_user_id = Tools.TypeHelper.ObjectToInt(HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
            //int login_user_role_id = Tools.TypeHelper.ObjectToInt(HttpContext.User.FindFirst(GlobalKeyConfig.Admin_Claim_RoleID).Value);
            bool login_user_role_super = Tools.TypeHelper.ObjectToBool(HttpContext.User.FindFirst(GlobalKeyConfig.Admin_Claim_IsSuperAdminRole).Value);
            //return new BLL.BLLSysRoute(_db_context).CheckAdminPower(login_user_role_id, login_user_role_super, page_key, is_post);
            if (login_user_role_super) return true;
            return User.HasClaim(page_key, is_post.ToString());
        }

        /// <summary>
        /// 获取当前控制器的路由
        /// </summary>
        /// <returns></returns>
        protected string GetControllerRoutePath()
        {
            string controller = RouteData.Values["controller"].ToString();
            string action = RouteData.Values["action"].ToString();
            return controller + "/" + action;
        }

        /// <summary>
        /// 分页Cookie Key
        /// </summary>
        /// <returns></returns>
        protected string CookieKey_PageSize()
        {
            return "admin" + GetControllerRoutePath().ToLower() + "Page";
        }

        /// <summary>
        /// 排序Cookie Key
        /// <returns></returns>
        protected string CookieKey_OrderBy()
        {
            return "admin" + GetControllerRoutePath().ToLower() + "Order";
        }


        #region Cookie操作

        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        protected void SetCookies(string key, string value, int minutes = 30)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }
        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        protected void DeleteCookies(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        protected string GetCookies(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }

        #endregion
    }
}
