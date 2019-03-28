using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class SysLogController : BaseAdminController
    {

        private readonly ILogger<SysLogController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="appkeys"></param>
        public SysLogController(ILoggerFactory loggerFactory, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = loggerFactory.CreateLogger<SysLogController>();
            _config_basic = appkeys.Value;
        }

        [AdminPermission("日志", "侧栏显示系统日志")]
        public ActionResult Log()
        {
            return Content("没有返回值");
        }

        /// <summary>
        /// 系统异常日志列表
        /// </summary>
        /// <returns></returns>
        [AdminPermission("日志", "系统异常日志列表")]
        public ActionResult LogException()
        {
            BLL.DynamicBLL<Entity.SysLogException> bll = new BLL.DynamicBLL<Entity.SysLogException>(_db_context);
            ViewData["CanDel"] = CheckAdminPower("SysLog/DelException", true);
            return View(bll.GetList(10, "", "AddTime desc", "ID>@0", "0"));
        }

        /// <summary>
        /// 系统操作日志列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        [AdminPermission("日志", "系统操作日志列表")]
        public async Task<IActionResult> LogMethod(int page = 1, int type = 0, string word = "")
        {

            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem() { Text = "所有日志", Value = "0" });
            foreach (var item in Tools.EnumHelper.BEnumToDictionary(typeof(Entity.SysLogMethodType)))
            {
                string text = Tools.EnumHelper.GetDescription((Entity.SysLogMethodType)item.Key);
                typeList.Add(new SelectListItem() { Text = text, Value = item.Key.ToString() });
            }
            ViewData["LogMethod_Type"] = typeList;

            var CookieKeyPageSize = CookieKey_PageSize();
            ViewData["CookieKey_PageSize"] = CookieKeyPageSize;
            var CookieKeyOrderBy = CookieKey_OrderBy();
            ViewData["CookieKey_OrderBy"] = CookieKeyOrderBy;

            word = Tools.WebHelper.UrlDecode(word);
            Models.ViewModelLogMethod response_model = new Models.ViewModelLogMethod();
            response_model.page = page;
            response_model.word = word;
            //获取每页大小的Cookie
            response_model.page_size = Tools.TypeHelper.ObjectToInt(GetCookies(CookieKeyPageSize), GlobalKeyConfig.Admin_Default_PageSize);

            string OrderBy = GetCookies(CookieKeyOrderBy);
            if (string.IsNullOrWhiteSpace(OrderBy)) OrderBy = "AddTime desc";
            ViewData["OrderBy"] = OrderBy;

            BLL.DynamicBLL<Entity.SysLogMethod> bll = new BLL.DynamicBLL<Entity.SysLogMethod>(_db_context);
            var queryWhere = "ID > 0 ";

            if (type != 0)
                queryWhere += $" AND Type={type.ToString()}";

            if (!string.IsNullOrWhiteSpace(word))
                queryWhere += $" AND Detail.Contains(\"{word}\")";

            var db_data = await bll.GetPagedListAsnyc(page, response_model.page_size, "SysUser", OrderBy, queryWhere);
            response_model.DataList = db_data.data;
            response_model.total = db_data.row_count;
            response_model.total_page = db_data.page_count;
            ViewData["CanDel"] = CheckAdminPower("SysLog/DelMethod", true);
            return View(response_model);
        }


        /// <summary>
        /// 删除异常日志
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminPermissionAttribute("日志", "删除异常日志")]
        public async Task<JsonResult> DelException(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return ResultBasicString(0, "缺少参数", "");
            BLL.DynamicBLL<Entity.SysLogException> bll = new BLL.DynamicBLL<Entity.SysLogException>(_db_context);
            if ("all".Equals(ids.ToLower())) await bll.DelAsync("ID>@0", "0");
            else await bll.DelByIdsAsync(ids);
            return ResultBasicString(1, "操作成功", "");
        }

        /// <summary>
        /// 删除删除操作日志
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminPermissionAttribute("日志", "删除操作日志")]
        public async Task<JsonResult> DelMethod(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) { return ResultBasicString(0, "缺少参数", ""); }
            BLL.DynamicBLL<Entity.SysLogMethod> bll = new BLL.DynamicBLL<Entity.SysLogMethod>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(ids.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(ids);
            return ResultBasicString(1, "删除成功", "");
        }

    }

}