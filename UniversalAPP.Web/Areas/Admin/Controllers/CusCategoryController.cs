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
    public class CusCategoryController : BaseAdminController
    {

        private readonly ILogger<CusCategoryController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appkeys"></param>
        public CusCategoryController(ILogger<CusCategoryController> logger, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = logger;
            _config_basic = appkeys.Value;
        }

        [AdminPermissionAttribute("分类管理", "分类管理首页")]
        public IActionResult Index()
        {
            return View(LoadCategory());
        }

        [AdminPermissionAttribute("分类管理", "删除分类")]
        [HttpPost]
        public async Task<JsonResult> Del(string ids)
        {
            int id = Tools.TypeHelper.ObjectToInt(ids);
            if (id <= 0) return ResultBasicString(0, "非法参数", "");
            string str = new BLL.BLLCusCategory(_db_context).GetChildIDStr(id);
            if (string.IsNullOrWhiteSpace(str)) return ResultBasicString(0, "找不到相关数据", "");

            BLL.DynamicBLL<Entity.CusCategory> bll = new BLL.DynamicBLL<Entity.CusCategory>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(str.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(str);
            
            return ResultBasicString(1, "删除成功", "");
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [AdminPermissionAttribute("分类管理", "分类管理编辑页面")]
        public async Task<ActionResult> Edit(int? id)
        {
            BLL.DynamicBLL<Entity.CusCategory> bll = new BLL.DynamicBLL<Entity.CusCategory>(_db_context);
            LoadCategorySelect();
            Entity.CusCategory entity = new Entity.CusCategory();
            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            if (num != 0)
            {
                entity = await bll.GetModelAsync("", "ID ASC", "ID=@0", num.ToString());
                if (entity == null)
                {
                    return PromptView("/admin/CusCategory", "Not Found", "信息不存在或已被删除", 5);
                }
            }
            return View(entity);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminPermissionAttribute("分类管理", "保存分类管理编辑信息")]
        public async Task<ActionResult> Edit(Entity.CusCategory entity)
        {
            var isAdd = entity.ID == 0 ? true : false;
            LoadCategorySelect();
            BLL.DynamicBLL<Entity.CusCategory> bll = new BLL.DynamicBLL<Entity.CusCategory>(_db_context);

            //数据验证
            if (!isAdd)
            {
                //如果要编辑的用户不存在
                if (!bll.Exists("ID=@0", entity.ID.ToString()))
                {
                    return PromptView("/admin/CusCategory", "Not Found", "信息不存在或已被删除", 5);
                }
            }

            if (ModelState.IsValid)
            {
                //添加
                if (entity.ID == 0)
                {
                    await new BLL.BLLCusCategory(_db_context).Add(entity);

                }
                else //修改
                {
                    await new BLL.BLLCusCategory(_db_context).Modify(entity);
                }

                return PromptView("/admin/CusCategory", "Success", "操作成功", 5);
            }
            else
                return View(entity);
        }

        /// <summary>
        /// 加载所有分类
        /// </summary>
        /// <returns></returns>
        public List<Entity.CusCategory> LoadCategory()
        {
            BLL.DynamicBLL<Entity.CusCategory> bll = new BLL.DynamicBLL<Entity.CusCategory>(_db_context);
            var oldData = bll.GetList(100, "", "Weight DESC", "ID>@0", "0");
            List<Entity.CusCategory> newData = new List<Entity.CusCategory>();
            GetChilds(oldData, newData, null);
            foreach (var item in newData)
            {
                item.Title = Tools.StringHelper.StringOfChar(item.Depth - 1, "&nbsp;&nbsp;") + "├ " + Tools.StringHelper.StringOfChar(item.Depth - 1, "&nbsp;&nbsp;&nbsp;&nbsp;") + item.Title;
            }
            return newData;
        }

        /// <summary>
        /// 加载所有分类select
        /// </summary>
        /// <returns></returns>
        public void LoadCategorySelect()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "选择父类", Value = "0" });
            BLL.DynamicBLL<Entity.CusCategory> bll = new BLL.DynamicBLL<Entity.CusCategory>(_db_context);
            var oldData = bll.GetList(100, "", "Weight DESC", "ID>@0", "0");
            List<Entity.CusCategory> newData = new List<Entity.CusCategory>();
            GetChilds(oldData, newData, null);
            foreach (var item in newData)
            {
                string text = Tools.StringHelper.StringOfChar(item.Depth - 1, "|--") + Tools.StringHelper.StringOfChar(item.Depth - 1, "|--") + item.Title;
                selectList.Add(new SelectListItem() { Text = text, Value = item.ID.ToString() });
            }
            ViewData["CategoryList"] = selectList;
        }

        private void GetChilds(List<Entity.CusCategory> oldData, List<Entity.CusCategory> newData, int? pid)
        {

            List<Entity.CusCategory> list = new List<Entity.CusCategory>();
            if (pid == null)
                list = oldData.Where(p => p.PID == null).ToList();
            else
                list = oldData.Where(p => p.PID == pid).ToList();
            foreach (var item in list)
            {
                Entity.CusCategory entity = new Entity.CusCategory();
                entity.AddTime = item.AddTime;
                entity.Depth = item.Depth;
                entity.ID = item.ID;
                entity.PID = item.PID;
                entity.Weight = item.Weight;
                entity.Status = item.Status;
                entity.Title = item.Title;
                newData.Add(entity);
                this.GetChilds(oldData, newData, item.ID);
            }
        }

    }
}