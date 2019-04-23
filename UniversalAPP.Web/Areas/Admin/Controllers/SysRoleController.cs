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
    public class SysRoleController : BaseAdminController
    {

        private readonly ILogger<SysRoleController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appkeys"></param>
        public SysRoleController(ILogger<SysRoleController> logger, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = logger;
            _config_basic = appkeys.Value;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        [AdminPermissionAttribute("后台用户组", "后台用户组列表")]
        public async Task<IActionResult> Index(int page = 1, string word = "")
        {
            var CookieKeyPageSize = CookieKey_PageSize();
            ViewData["CookieKey_PageSize"] = CookieKeyPageSize;
            var CookieKeyOrderBy = CookieKey_OrderBy();
            ViewData["CookieKey_OrderBy"] = CookieKeyOrderBy;

            word = Tools.WebHelper.UrlDecode(word);
            Models.ViewModelSysRoleList response_model = new Models.ViewModelSysRoleList();
            response_model.page = page;
            response_model.word = word;
            //获取每页大小的Cookie
            response_model.page_size = Tools.TypeHelper.ObjectToInt(GetCookies(CookieKeyPageSize), GlobalKeyConfig.Admin_Default_PageSize);
            string OrderBy = GetCookies(CookieKeyOrderBy);
            if (string.IsNullOrWhiteSpace(OrderBy)) OrderBy = "AddTime desc";
            ViewData["OrderBy"] = OrderBy;

            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            var queryWhere = "ID > 0 ";

            if (!string.IsNullOrWhiteSpace(word))
                queryWhere += $" AND RoleName.Contains(\"{word}\")";

            var db_data = await bll.GetPagedListAsnyc(page, response_model.page_size, "", OrderBy, queryWhere);
            response_model.DataList = db_data.data;
            response_model.total = db_data.row_count;
            response_model.total_page = db_data.page_count;

            ViewData["CanEdit"] = CheckAdminPower("sysrole/edit", true);
            ViewData["CanDel"] = CheckAdminPower("sysrole/del", true);

            return View(response_model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminPermissionAttribute("后台用户组", "删除后台用户组")]
        public async Task<JsonResult> Del(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) { return ResultBasicString(0, "缺少参数", ""); }
            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(ids.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(ids);
            if (ss > 0) AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Delete, $"删除用户组{ids}");
            return ResultBasicString(1, "删除成功", "");
        }

        /// <summary>
        /// 更新权限数据
        /// </summary>
        /// <returns></returns>
        //[AdminPermissionAttribute("其他", "更新权限数据")]
        [HttpPost]
        public JsonResult UpdateRoute()
        {
            List<Models.ModelRoute> route_list = new List<Models.ModelRoute>();

            #region 反射获取所有的控制路由

            System.Reflection.Assembly assembly = this.GetType().Assembly;

            foreach (var type in assembly.ExportedTypes)
            {
                System.Reflection.MemberInfo[] properties = type.GetMembers();
                foreach (var item in properties)
                {
                    string controllerName = item.ReflectedType.Name.Replace("Controller", "").ToString();
                    string actionName = item.Name.ToString();
                    //访问路由
                    string route_map = controllerName.ToLower() + "/" + actionName.ToLower();
                    //是否是HttpPost请求
                    bool IsHttpPost = item.GetCustomAttributes(typeof(HttpPostAttribute), true).Count() > 0 ? true : false;

                    object[] attrs = item.GetCustomAttributes(typeof(AdminPermissionAttribute), true);
                    if (attrs.Length == 1)
                    {
                        AdminPermissionAttribute attr = (AdminPermissionAttribute)attrs[0];
                        route_list.Add(new Models.ModelRoute
                        {
                            Tag = attr.Tag,
                            Desc = attr.Desc,
                            IsPost = IsHttpPost,
                            Route = route_map
                        });
                    }
                }
            }
            #endregion


            BLL.DynamicBLL<Entity.SysRoute> bll = new BLL.DynamicBLL<Entity.SysRoute>(_db_context);
            var db_list = bll.GetList(100000, "", "ID ASC", "ID > @0", "0");

            foreach (var item in db_list)
            {
                var entity = route_list.Where(p => p.IsPost == item.IsPost && p.Route == item.Route).FirstOrDefault();
                //如果数据库对应程序中不存在，则删除数据库里的
                if (entity == null)
                {
                    bll.Del("ID=@0", item.ID.ToString());

                }
                else
                {
                    //否则修改数据库里的DES之类的辅助说明              
                    item.Desc = entity.Desc;
                    item.Tag = entity.Tag;
                    bll.Modify(item);
                }
            }

            foreach (var item in route_list)
            {
                var entity = bll.GetModel("", "ID ASC", "IsPost=@0 AND Route=@1", item.IsPost.ToString(), item.Route);
                if (entity == null)
                {
                    var route = new Entity.SysRoute();
                    route.Desc = item.Desc;
                    route.IsPost = item.IsPost;
                    route.Route = item.Route;
                    route.Tag = item.Tag;
                    bll.Add(route);
                }
            }
            AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Modify, "更新权限路由");
            return ResultBasicString(1, "更新成功", "");
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AdminPermissionAttribute("后台用户组", "后台用户组编辑页面")]
        public async Task<IActionResult> Edit(int? id)
        {
            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            if (id == null)
                await GetTree();
            else
                await GetTree(Tools.TypeHelper.ObjectToInt(id, 0));
            Entity.SysRole entity = new Entity.SysRole();
            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            if (num != 0)
            {
                entity = await bll.GetModelAsync("", "ID ASC", "ID=@0", num.ToString());
                if (entity == null)
                {
                    return PromptView("/admin/SysRole", "Not Found", "信息不存在或已被删除", 5);
                }
            }
            return View(entity);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AdminPermissionAttribute("后台用户组", "保存后台用户组编辑的信息")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Entity.SysRole entity)
        {
            var isAdd = entity.ID == 0 ? true : false;
            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            await GetTree(entity.ID);

            var qx = Request.Form["hid_qx"].ToString();
            //数据验证
            if (isAdd)
            {
                if (bll.Exists("RoleName=@0", entity.RoleName))
                    ModelState.AddModelError("RoleName", "该组名已存在");
            }
            else
            {
                if (!bll.Exists("ID=@0", entity.ID.ToString()))
                    return PromptView("/admin/SysRole", "Not Found", "该组不存在或已被删除", 5);

                var old_entity = bll.GetModel("", "ID ASC", "ID=@0", entity.ID.ToString());
                //验证组名是否存在
                if (old_entity.RoleName != entity.RoleName)
                {
                    if (bll.Exists("RoleName=@0", entity.RoleName))
                        ModelState.AddModelError("RoleName", "该组名已存在");
                }
            }

            if (ModelState.IsValid)
            {
                BLL.BLLSysRole bll_role = new BLL.BLLSysRole(_db_context);
                if (entity.ID == 0)//添加
                {
                    await bll_role.Add(entity, qx);
                    AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Add, $"添加用户组{entity.RoleName}");
                }
                else //修改
                {
                    await bll_role.Modify(entity, qx);
                    AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Modify, $"修改用户组{entity.RoleName}");
                }

                return PromptView("/admin/SysRole", "Success", "操作成功", 5);
            }
            else
                return View(entity);

        }


        /// <summary>
        /// 获取树数据
        /// </summary>
        /// <param name="id">当前组ID，没有传0</param>
        private async Task GetTree(int id = 0)
        {
            BLL.DynamicBLL<Entity.SysRole> bll_route = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            BLL.DynamicBLL<Entity.SysRoleRoute> bll_role_route = new BLL.DynamicBLL<Entity.SysRoleRoute>(_db_context);
            Entity.SysRole role = null;
            if (id != 0)
                role = bll_route.GetModel("", "ID ASC", "ID=@0", id.ToString());

            List<Models.ViewModelTree> list = new List<Models.ViewModelTree>();
            var route_group = await new BLL.BLLSysRoute(_db_context).GetListGroupByTag();
            for (int i = 0; i < route_group.Count; i++)
            {
                int top_id = i + 10000;
                Models.ViewModelTree model = new Models.ViewModelTree();
                model.id = top_id;
                model.name = route_group[i].Key;
                model.open = i < 4 ? true : false;
                model.pId = 0;
                list.Add(model);
                foreach (var item in route_group[i].ToList())
                {
                    Models.ViewModelTree model2 = new Models.ViewModelTree();
                    model2.id = item.ID;
                    model2.name = item.Desc;
                    model2.open = false;
                    model2.pId = top_id;
                    if (role != null)
                        model2.is_checked = bll_role_route.Exists("SysRoleID=@0 AND SysRouteID=@1", role.ID.ToString(), item.ID.ToString());

                    list.Add(model2);
                }

            }

            ViewData["Tree"] = Tools.JsonHelper.ToJson(list).Replace("is_checked", "checked");
        }

    }
}