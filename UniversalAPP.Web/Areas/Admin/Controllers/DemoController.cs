using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class DemoController : BaseAdminController
    {

        private readonly ILogger<DemoController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appkeys"></param>
        public DemoController(ILogger<DemoController> logger, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = logger;
            _config_basic = appkeys.Value;
            
        }


        public async Task<IActionResult> Index(int page = 1, string word = "")
        {
            word = Tools.WebHelper.UrlDecode(word);
            Models.ViewModelDemoList response_model = new Models.ViewModelDemoList();
            response_model.page = page;
            response_model.word = word;
            //获取每页大小的Cookie
            response_model.page_size = Tools.TypeHelper.ObjectToInt(GetCookies(CookieKey_PageSize()), GlobalKeyConfig.Admin_Default_PageSize);
            string OrderBy = GetCookies(CookieKey_OrderBy());
            if (string.IsNullOrWhiteSpace(OrderBy)) OrderBy = "AddTime desc";
            ViewData["OrderBy"] = OrderBy;

            BLL.DynamicBLL<Entity.Demo> bll = new BLL.DynamicBLL<Entity.Demo>(_db_context);
            var queryWhere = "ID > 0 ";

            if (!string.IsNullOrWhiteSpace(word))
                queryWhere += $" AND Title.Contains(\"{word}\")";

            var db_data = await bll.GetPagedListAsnyc(page, response_model.page_size, "", OrderBy, queryWhere);
            response_model.DataList = db_data.data;
            response_model.total = db_data.row_count;
            response_model.total_page = db_data.page_count;
            return View(response_model);
        }

        [HttpPost]
        public async Task<JsonResult> Del(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) { return ResultBasicString(0, "缺少参数", ""); }
            BLL.DynamicBLL<Entity.SysRole> bll = new BLL.DynamicBLL<Entity.SysRole>(_db_context);
            //var id_list = Array.ConvertAll<string, int>(ids.Split(','), int.Parse);
            var ss = await bll.DelByIdsAsync(ids);
            return ResultBasicString(1, "删除成功", "");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            BLL.BLLDemo bll = new BLL.BLLDemo(_db_context);
            Entity.Demo entity = new Entity.Demo();
            entity.Depts.Add(new Entity.DemoDept()
            {
                ImgUrl = "",
                Num = 0,
                Title = ""
            });

            int num = Tools.TypeHelper.ObjectToInt(id, 0);
            if (num != 0)
            {
                entity = await bll.GetModel(num);
                if (entity == null)
                {
                    return PromptView("/admin/demo", "Not Found", "信息不存在或已被删除", 5);
                }
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Entity.Demo entity)
        {
            var isAdd = entity.ID == 0 ? true : false;
            BLL.DynamicBLL<Entity.Demo> bll = new BLL.DynamicBLL<Entity.Demo>(_db_context);
            string str_albums = Request.Form["StrAlbums"].ToString();
            if (!isAdd)
            {
                if (!bll.Exists("ID=@0", entity.ID.ToString()))
                {
                    return PromptView("/admin/demo", "Not Found", "信息不存在或已被删除", 5);
                }
            }
            int login_user_id = Tools.TypeHelper.ObjectToInt(User.FindFirst(ClaimTypes.Sid).Value);
            if (ModelState.IsValid)
            {
                //添加
                if (entity.ID == 0)
                {
                    entity.AddTime = DateTime.Now;
                    entity.LastUpdateTime = DateTime.Now;
                    entity.AddUserID = login_user_id;
                    entity.LastUpdateUserID = login_user_id;
                    bll.Add(entity);

                }
                else //修改
                {
                    entity.LastUpdateTime = DateTime.Now;
                    entity.LastUpdateUserID = login_user_id;
                    new BLL.BLLDemo(_db_context).Modify(entity);
                }

                return PromptView("/admin/demo", "Success", "操作成功", 5);
            }
            else
            {
                return View(entity);
            }
            
        }
        
    }
}