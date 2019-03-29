using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Swashbuckle.AspNetCore.Swagger;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }



        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region 初始化公共DI需要
            services.AddHttpContextAccessor();
            #endregion

            #region 配置文件

            //注入配置文件
            services.Configure<Models.SiteBasicConfig>(Configuration.GetSection("SiteBasicConfig"));
            //用户机密
            //var secrets_val = Configuration.GetSection("Email:Pwd").Value;

            #endregion

            #region 数据库

            services.AddDbContext<EFCore.EFDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), p => p.UseRowNumberForPaging()));

            #endregion

            #region 身份验证

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

            #endregion

            #region MVC
            
            //添加MVC及过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                options.Filters.Add(typeof(CustomAuthorizationFilterAttribute));
                //options.Filters.Add(typeof(SimpleResourceFilterAttribute));
                //options.Filters.Add(typeof(SimpleActionFilterAttribute));
                //options.Filters.Add(typeof(SimpleResultFilterAttribute));
            }).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
            services.AddSession();

            #endregion

            #region Cookie GDPR

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; //设置为False为禁用这个规范，问题：https://blog.csdn.net/shiershilian/article/details/80876803
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #endregion

            #region Hangfire计划任务

            ////Hangfire计划任务
            //services.AddHangfire(p => p.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));


            #endregion

            #region Swagger API文档生成
            ////注册Swagger生成器，定义一个和多个Swagger 文档
            ////属性>生成>XML文档文件 bin\debug\netcoreapp2.2\Swagger.xml
            ////属性>生成>禁止显示警告 1701;1702
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "UniversalAPP Web API Document", Version = "v1" });
            //    // 为 Swagger JSON and UI设置xml文档注释路径
            //    var basePath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
            //    var xmlPath = System.IO.Path.Combine(basePath, "Swagger.xml");
            //    c.IncludeXmlComments(xmlPath);
            //});

            #endregion

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            #region 初始化公共DI需要
            ServiceLocator.Instance = app.ApplicationServices;
            #endregion

            #region 判断运行环境

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //开发环境中接口验证不开启
                //app.UseWhen(x => (x.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)), builder =>
                //    {
                //        builder.UseAPIAuthMiddleware();
                //    });

                //app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            #endregion
            
            #region MVC相关

            app.UseHttpsRedirection();
            app.UseAuthentication();
            //获取IP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //为了在nginx上获取用户端真实IP添加此代码
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //app.UseCookiePolicy();//增加对于GDPR政策的支持
            app.UseSession();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            #endregion

            #region 其他公共

            //图片不存在时返回默认图片
            app.UseDefaultImage(defaultImagePath: Configuration.GetSection("defaultImagePath").Value);

            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new CustomTraceListener());

            #endregion

            #region Hangfire计划任务

            ////Hangfire计划任务
            //app.UseHangfireServer();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });

            #endregion

            #region Swagger API文档生成

            ////启用中间件服务生成Swagger作为JSON终结点
            //app.UseSwagger();
            ////启用中间件服务对swagger-ui，指定Swagger JSON终结点
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniversalAPP Web API Document");
            //});

            #endregion
        }
    }
}
