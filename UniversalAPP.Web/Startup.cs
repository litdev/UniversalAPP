using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UniversalAPP.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EFCore.EFDBContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), p => p.UseRowNumberForPaging()));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.LoginPath = "/Admin/Home/Login";
                option.LogoutPath = "/Admin/Home/LoginOut";
            });
            #region 添加自定义验证Scheme

            //再添加一个验证Scheme
            //.AddCookie(CustomerAuthorizeAttribute.CustomerAuthenticationScheme, option =>
            // {
            //     option.LoginPath = new PathString("/Account/Signin");
            //     option.AccessDeniedPath = new PathString("/Error/Forbidden");
            // });
            //验证登录状态：控制器或Action加上属性[CustomerAuthorize]
            //获取自定义的HttpContext.User
            //var auth = await HttpContext.AuthenticateAsync(CustomerAuthorizeAttribute.CustomerAuthenticationScheme);
            //if (auth.Succeeded) var user_identity = auth.Principal.Identity;
            //退出登录：await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CustomerAuthorizeAttribute.CustomerAuthenticationScheme);

            #endregion

            //注入配置文件
            services.Configure<Models.SiteConfig>(Configuration.GetSection("SiteConfig"));

            //添加MVC及过滤器
            services.AddMvc(options=> {
                options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                options.Filters.Add(typeof(CustomAuthorizationFilterAttribute));
                //options.Filters.Add(typeof(SimpleResourceFilterAttribute));
                //options.Filters.Add(typeof(SimpleActionFilterAttribute));
                //options.Filters.Add(typeof(SimpleResultFilterAttribute));
            });
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                //开发环境中接口验证不开启
                //app.UseWhen(x => (x.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)), builder =>
                //    {
                //        builder.UseAPIAuthMiddleware();
                //    });
                
                //app.UseExceptionHandler("/Home/Error");
            }
            //验证
            app.UseAuthentication();
            
            app.UseSession();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name:"areaRoute",
                    template:"{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
