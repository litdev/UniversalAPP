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
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Web.Models.SiteConfig _config;
        private readonly EFCore.EFDBContext _db_context;
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment environment,ILoggerFactory loggerFactory, IOptionsSnapshot<Web.Models.SiteConfig> appkeys, EFCore.EFDBContext context)
        {
            _env = environment;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _config = appkeys.Value;
            _db_context = context;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }

        /// <summary>
        /// 登录处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
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
            var entity_user = await bll.GetModelAsync("", "ID ASC", "UserName=@0 and Password=@1", viewModel.user_name, input_pwd);
            if (entity_user == null)
            {
                ModelState.AddModelError("user_name", "用户名不存在或密码错误");
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
            identity.AddClaim(new Claim(ClaimTypes.Role, entity_user.SysRoleID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.UserData, entity_user.AvatarOrDefault));
            var expires_time = new AuthenticationProperties();
            if (viewModel.is_rember)
            {
                expires_time.IsPersistent = true;
                expires_time.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }
            await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, new ClaimsPrincipal(identity), expires_time);
            await bll.ExecuteSqlCommandAsync($"Update SysUser set LastLoginTime=getdate() where ID ={entity_user.ID}");
            return RedirectToAction("Index");
        }

        [Authorize]
        [AdminPermission("其他", "后台管理首页")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Center()
        {
            return View();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Authorize]
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