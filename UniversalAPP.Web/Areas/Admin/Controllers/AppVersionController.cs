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
    public class AppVersionController : BaseAdminController
    {
        [AdminPermissionAttribute("App版本", "APP版本首页")]
        public async Task<IActionResult> Index(int page = 1, int platform = 0)
        {
            var CookieKeyPageSize = CookieKey_PageSize();
            ViewData["CookieKey_PageSize"] = CookieKeyPageSize;
            var CookieKeyOrderBy = CookieKey_OrderBy();
            ViewData["CookieKey_OrderBy"] = CookieKeyOrderBy;

            LoadPlatform();

            Models.ViewModelAppVersion response_model = new Models.ViewModelAppVersion();
            response_model.page = page;
            response_model.platform = platform;

            //获取每页大小的Cookie
            response_model.page_size = Tools.TypeHelper.ObjectToInt(GetCookies(CookieKeyPageSize), GlobalKeyConfig.Admin_Default_PageSize);
            string OrderBy = GetCookies(CookieKeyOrderBy);
            if (string.IsNullOrWhiteSpace(OrderBy)) OrderBy = "AddTime desc";
            ViewData["OrderBy"] = OrderBy;

            BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);
            var queryWhere = "ID > 0 ";

            if (platform != 0)
                queryWhere += $" AND Platforms=(\"{platform}\")";

            var db_data = await bll.GetPagedListAsnyc(page, response_model.page_size, "", OrderBy, queryWhere);
            response_model.DataList = db_data.data;
            response_model.total = db_data.row_count;
            response_model.total_page = db_data.page_count;
            return View(response_model);
        }

        [HttpPost]
        [AdminPermission("App版本", "删除版本")]
        public async Task<JsonResult> Del(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) { return ResultBasicString(0, "缺少参数", ""); }
            BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(ids.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(ids);
            return ResultBasicString(1, "删除成功", "");
        }

        /// <summary>
        /// 编辑安卓
        /// </summary>
        /// <returns></returns>
        [AdminPermissionAttribute("App版本", "安卓版本编辑页面")]
        public async Task<IActionResult> EditAndroid(int? id)
        {
            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            Entity.AppVersion entity = new Entity.AppVersion();
            if (num != 0)
            {
                BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);
                entity = await bll.GetModelAsync("", "ID ASC", "ID=@0", num.ToString());
                if (entity == null)
                    return PromptView("/admin/AppVersion", "Not Found", "信息不存在或已被删除", 5);
                if (entity.Platforms != Entity.APPVersionPlatforms.Android)
                    return PromptView("/admin/AppVersion", "数据非法", "此信息非安卓版本", 5);

            }
            return View(entity);
        }

        /// <summary>
        /// 保存安卓编辑信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AdminPermission("App版本", "保存安卓编辑信息")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAndroid(Entity.AppVersion entity)
        {
            var isAdd = entity.ID == 0 ? true : false;
            BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);

            entity.APPType = Entity.APPVersionType.Standard;
            entity.Platforms = Entity.APPVersionPlatforms.Android;
            entity.LogoImg = "NULL";
            //数据验证
            if (isAdd)
            {
                //判断版本是否存在
                if (bll.Exists("Platforms=@0 AND APPType=@1 AND Version=@2", Entity.APPVersionPlatforms.Android.ToString(), Entity.APPVersionType.Standard.ToString(), entity.Version))
                {
                    ModelState.AddModelError("Content", "该版本存在");
                }

            }
            else
            {
                if (!bll.Exists("ID=@0", entity.ID.ToString()))
                {
                    return PromptView("/admin/AppVersion", "Not Found", "信息不存在或已被删除", 5);
                }
            }
            if (string.IsNullOrWhiteSpace(entity.DownUrl)) ModelState.AddModelError("DownUrl", "APK包不能为空");
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(entity.DownUrl, false)) ModelState.AddModelError("DownUrl", "APK文件不存在");
            else
            {
                entity.MD5 = fileHelper.GetMD5HashFromFile(entity.DownUrl);
                entity.Size = fileHelper.GetFileSize(entity.DownUrl);
            }
            entity.LinkUrl = entity.DownUrl;
            ModelState.Remove("MD5");
            if (ModelState.IsValid)
            {
                //添加
                if (entity.ID == 0)
                {
                    var new_ver = bll.GetModel("", "VersionCode DESC", "Platforms=@0 AND APPType=@1", Entity.APPVersionPlatforms.Android.ToString(), entity.APPType.ToString());
                    entity.VersionCode = new_ver == null ? 1 : new_ver.VersionCode + 1;

                    await bll.AddAsync(entity);

                }
                else //修改
                    await bll.ModifyAsync(entity);

                return PromptView("/admin/AppVersion", "Success", "操作成功", 5);
            }
            else
            {
                string error = string.Empty;
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        error = state.Errors.First().ErrorMessage;
                        break;
                    }
                }
                ModelState.AddModelError("Version", error);
                return View(entity);
            }
        }


        /// <summary>
        /// 编辑IOS
        /// </summary>
        /// <returns></returns>
        [AdminPermissionAttribute("App版本", "IOS版本编辑页面")]
        public async Task<IActionResult> EditIOS(int? id)
        {
            LoadPlatform();
            Entity.AppVersion entity = new Entity.AppVersion();
            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);

            if (num != 0)
            {
                entity = await bll.GetModelAsync("", "ID ASC", "ID=@0", num.ToString());
                if (entity == null)
                    return PromptView("/admin/AppVersion", "Not Found", "信息不存在或已被删除", 5);
                if (entity.Platforms != Entity.APPVersionPlatforms.IOS)
                    return PromptView("/admin/AppVersion", "数据非法", "此信息非IOS版本", 5);
            }

            return View(entity);
        }

        /// <summary>
        /// 保存IOS编辑信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AdminPermission("App版本", "保存苹果编辑信息")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIOS(Entity.AppVersion entity)
        {
            var isAdd = entity.ID == 0 ? true : false;
            LoadPlatform();
            BLL.DynamicBLL<Entity.AppVersion> bll = new BLL.DynamicBLL<Entity.AppVersion>(_db_context);

            entity.Platforms = Entity.APPVersionPlatforms.IOS;
            entity.LogoImg = "null";

            if (string.IsNullOrWhiteSpace(entity.DownUrl)) ModelState.AddModelError("DownUrl", "APK包不能为空");
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(entity.DownUrl, false)) ModelState.AddModelError("DownUrl", "APK文件不存在");
            else
            {
                entity.MD5 = fileHelper.GetMD5HashFromFile(entity.DownUrl);
                entity.Size = fileHelper.GetFileSize(entity.DownUrl);
            }

            ModelState.Remove("LogoImg");
            ModelState.Remove("MD5");



            //数据验证
            if (isAdd)
            {
                //判断版本是否存在
                if (bll.Exists("Platforms=@0 AND APPType=@1 AND Version=@2", Entity.APPVersionPlatforms.IOS.ToString(), entity.APPType.ToString(), entity.Version))
                {
                    ModelState.AddModelError("Version", "该版本存在");
                }

            }
            else
            {
                if (bll.Exists("ID=@0", entity.ID.ToString()))
                {
                    return PromptView("/admin/AppVersion", "Not Found", "信息不存在或已被删除", 5);
                }
            }

            if (ModelState.IsValid)
            {
                //添加
                if (entity.ID == 0)
                {
                    var new_ver = bll.GetModel("", "VersionCode DESC", "Platforms=@0 AND APPType=@1", Entity.APPVersionPlatforms.IOS.ToString(), entity.APPType.ToString());
                    entity.VersionCode = new_ver == null ? 1 : new_ver.VersionCode + 1;

                    await bll.AddAsync(entity);

                }
                else //修改
                {
                    await bll.ModifyAsync(entity);
                }
                return PromptView("/admin/AppVersion", "Success", "操作成功", 5);
            }
            else
                return View(entity);
        }



        /// <summary>
        /// 加载平台列表
        /// </summary>
        private void LoadPlatform()
        {
            List<SelectListItem> platformList = new List<SelectListItem>();
            platformList.Add(new SelectListItem() { Text = "所有平台", Value = "0" });
            foreach (var item in Tools.EnumHelper.BEnumToDictionary(typeof(Entity.APPVersionPlatforms)))
            {
                string text = Tools.EnumHelper.GetDescription((Entity.APPVersionPlatforms)item.Key);
                platformList.Add(new SelectListItem() { Text = text, Value = item.Key.ToString() });
            }
            ViewData["PlatformsList"] = platformList;

            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem() { Text = "所有类别", Value = "0" });
            foreach (var item in Tools.EnumHelper.BEnumToDictionary(typeof(Entity.APPVersionType)))
            {
                string text = Tools.EnumHelper.GetDescription((Entity.APPVersionType)item.Key);
                typeList.Add(new SelectListItem() { Text = text, Value = item.Key.ToString() });
            }
            ViewData["TypeList"] = typeList;
        }
    }
}