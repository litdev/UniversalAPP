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
    public class SysUserController : BaseAdminController
    {
        private readonly ILogger<SysUserController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appkeys"></param>
        public SysUserController(ILogger<SysUserController> logger, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = logger;
            _config_basic = appkeys.Value;
        }


        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [AdminPermissionAttribute("后台用户", "后台用户列表")]
        public async Task<IActionResult> Index(int page = 1, int role = 0, string word = "")
        {
            var CookieKeyPageSize = CookieKey_PageSize();
            ViewData["CookieKey_PageSize"] = CookieKeyPageSize;
            var CookieKeyOrderBy = CookieKey_OrderBy();
            ViewData["CookieKey_OrderBy"] = CookieKeyOrderBy;

            word = Tools.WebHelper.UrlDecode(word);
            Models.ViewModelSysUserList response_model = new Models.ViewModelSysUserList();
            response_model.page = page;
            response_model.role = role;
            response_model.word = word;
            response_model.page_size = Tools.TypeHelper.ObjectToInt(GetCookies(CookieKeyPageSize), GlobalKeyConfig.Admin_Default_PageSize);
            string OrderBy = GetCookies(CookieKeyOrderBy);
            if (string.IsNullOrWhiteSpace(OrderBy)) OrderBy = "RegTime desc";
            ViewData["OrderBy"] = OrderBy;
            Load();

            BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(_db_context);
            var queryWhere = "ID > 0 ";

            if (role != 0)
                queryWhere += $" AND SysRoleID = {role} ";

            if (!string.IsNullOrWhiteSpace(word))
                queryWhere += $" AND (username.Contains(\"{word}\") OR UserName.Contains(\"{word}\"))";

            var db_data = await bll.GetPagedListAsnyc(page, response_model.page_size, "SysRole", OrderBy, queryWhere);
            response_model.DataList = db_data.data;
            response_model.total = db_data.row_count;
            response_model.total_page = db_data.page_count;
            return View(response_model);
        }


        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        [AdminPermissionAttribute("后台用户", "删除后台用户")]
        public async Task<JsonResult> Del(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) { return ResultBasicString(0, "缺少参数", ""); }
            BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(ids.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(ids);
            if (ss > 0) AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Delete, $"删除用户{ids}");
            return ResultBasicString(1, "删除成功", "");
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [AdminPermissionAttribute("后台用户", "后台用户编辑页面")]
        public async Task<IActionResult> Edit(int? id)
        {
            BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(_db_context);
            Load();
            Entity.SysUser entity = new Entity.SysUser();
            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            if (num != 0)
            {
                entity = await bll.GetModelAsync("", "ID ASC", "ID=@0", num.ToString());
                if (entity == null)
                {
                    return PromptView("/admin/SysUser", "Not Found", "信息不存在或已被删除", 5);
                }
            }
            return View(entity);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminPermissionAttribute("后台用户", "保存后台用户编辑信息")]
        public async Task<IActionResult> Edit(Entity.SysUser entity)
        {
            var isAdd = entity.ID == 0 ? true : false;

            BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(_db_context);
            Load();

            if (entity.SysRoleID == 0)
            {
                ModelState.AddModelError("SysRoleID", "请选择用户组");
            }

            //数据验证
            if (isAdd)
            {
                //判断用户名是否存在
                if (bll.Exists("UserName=@0", entity.UserName))
                {
                    ModelState.AddModelError("UserName", "该用户名已存在");
                }

            }
            else
            {
                //如果要编辑的用户不存在
                if (!bll.Exists("ID=@0", entity.ID.ToString()))
                {
                    return PromptView("/admin/SysUser", "Not Found", "信息不存在或已被删除", 5);
                }
                ModelState.Remove("UserName");
            }

            if (ModelState.IsValid)
            {
                //添加
                if (entity.ID == 0)
                {
                    entity.RegTime = DateTime.Now;
                    entity.Password = Tools.AESHelper.Encrypt(entity.Password, GlobalKeyConfig.AESKey, GlobalKeyConfig.AESIV);
                    entity.LastLoginTime = DateTime.Now;
                    await bll.AddAsync(entity);
                    AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Add, $"添加用户{entity.UserName}");
                }
                else //修改
                {
                    var entity_old = bll.GetModel("", "ID ASC", "ID=@0", entity.ID.ToString());
                    if (entity.Password != "litdev")
                        entity_old.Password = Tools.AESHelper.Encrypt(entity.Password, GlobalKeyConfig.AESKey, GlobalKeyConfig.AESIV);
                    entity_old.NickName = entity.NickName;
                    entity_old.Gender = entity.Gender;
                    entity_old.Status = entity.Status;
                    entity_old.Avatar = entity.Avatar;
                    entity_old.SysRoleID = entity.SysRoleID;
                    await bll.ModifyAsync(entity_old);
                    AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Modify, $"修改用户{entity.UserName}");
                }

                return PromptView("/admin/SysUser", "Success", "操作成功", 5);
            }
            else
                return View(entity);
        }

        /// <summary>
        /// 加载用户组
        /// </summary>
        private void Load()
        {
            List<SelectListItem> userRoleList = new List<SelectListItem>();
            userRoleList.Add(new SelectListItem() { Text = "全部组", Value = "0" });
            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            foreach (var item in bll.GetList(100, "", "ID ASC", "ID > 0"))
            {
                userRoleList.Add(new SelectListItem() { Text = item.RoleName, Value = item.ID.ToString() });
            }
            ViewData["sysuser_role"] = userRoleList;
        }
    }
}