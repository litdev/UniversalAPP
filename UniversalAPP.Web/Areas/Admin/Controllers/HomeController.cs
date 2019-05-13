using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;

namespace UniversalAPP.Web.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class HomeController : BaseAdminController
    {
        private readonly ILogger<HomeController> _logger;
        private Web.Models.SiteBasicConfig _config_basic;

        public HomeController(ILogger<HomeController> logger, IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _logger = logger;
            _config_basic = appkeys.Value;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet,AllowAnonymous]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }

        /// <summary>
        /// 登录处理
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Models.ViewModelLogin viewModel)
        {
            if (HttpContext.Session.GetString(GlobalKeyConfig.Session_Login_Fail_Total) != null)
            {
                if (Tools.TypeHelper.ObjectToInt(HttpContext.Session.GetString(GlobalKeyConfig.Session_Login_Fail_Total), 0) > 3)
                {
                    ModelState.AddModelError("user_name", "失败次数过多，重启浏览器后再试");
                    return View(viewModel);
                }
            }

            if (!ModelState.IsValid) return View(viewModel);

            string input_pwd = Tools.AESHelper.Encrypt(viewModel.password, GlobalKeyConfig.AESKey, GlobalKeyConfig.AESIV);
            BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(_db_context);
            var entity_user = await bll.GetModelAsync("SysRole", "ID ASC", "UserName=@0 and Password=@1", viewModel.user_name, input_pwd);
            if (entity_user == null)
            {
                ModelState.AddModelError("user_name", "用户名不存在或密码错误");
                return View(viewModel);
            }
            if(entity_user.SysRole == null)
            {
                ModelState.AddModelError("user_name", "用户组不存在");
                return View(viewModel);
            }
            if (!entity_user.Status)
            {
                ModelState.AddModelError("user_name", "用户已被禁用");
                return View(viewModel);
            }

            //用户标识
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Sid, entity_user.ID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, entity_user.UserName));
            identity.AddClaim(new Claim(GlobalKeyConfig.Admin_Claim_RoleID, entity_user.SysRoleID.ToString()));
            identity.AddClaim(new Claim(GlobalKeyConfig.Admin_Claim_IsSuperAdminRole, entity_user.SysRole.IsAdmin.ToString()));
            identity.AddClaim(new Claim(GlobalKeyConfig.Admin_Claim_Avatar, entity_user.AvatarOrDefault));
            if (!entity_user.SysRole.IsAdmin)
            {
                foreach (var item in new BLL.BLLSysRoute(_db_context).GetRoleRouteList(entity_user.SysRoleID))
                {
                    identity.AddClaim(new Claim(item.Route, item.IsPost.ToString()));
                }
            }

            var expires_time = new AuthenticationProperties();
            if (viewModel.is_rember)
            {
                expires_time.IsPersistent = true;
                expires_time.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }
            await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, new ClaimsPrincipal(identity), expires_time);
            await bll.ExecuteSqlCommandAsync($"Update SysUser set LastLoginTime=now() where ID ={entity_user.ID}");
            AddMethodLog(_config_basic.LogMethodInDB, Entity.SysLogMethodType.Login, "用户登录", entity_user.ID);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Center()
        {
            return View();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoginOut()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            return Redirect("/Admin/Home/Login");
        }

        /// <summary>
        /// 登陆错误次数+1
        /// </summary>
        private void LoginFileTimesAdd()
        {
            //登陆次数+1
            if (HttpContext.Session.GetInt32(GlobalKeyConfig.Session_Login_Fail_Total) == null)
                HttpContext.Session.SetInt32(GlobalKeyConfig.Session_Login_Fail_Total, 1);
            else
                HttpContext.Session.SetInt32(GlobalKeyConfig.Session_Login_Fail_Total, Tools.TypeHelper.ObjectToInt(HttpContext.Session.GetInt32(GlobalKeyConfig.Session_Login_Fail_Total), 0) + 1);
        }

    }
}