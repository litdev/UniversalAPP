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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.StaticFiles;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Api版本信息
        /// </summary>
        private IApiVersionDescriptionProvider provider;


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
                options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));

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
            //services.AddHangfire(x => x.UseStorage(new Hangfire.MySql.Core.MySqlStorage(Configuration.GetConnectionString("HangfireMySqlConnection"), new Hangfire.MySql.Core.MySqlStorageOptions() { TablePrefix = "Custom" })));

            #endregion

            #region Swagger API文档生成

            //添加版本控制
            services.AddApiVersioning(option =>
            {
                // 可选，为true时API返回支持的版本信息
                option.ReportApiVersions = true;
                // 不提供版本时，默认为1.0
                //option.AssumeDefaultVersionWhenUnspecified = false;
                // 请求中未指定版本时默认为1.0
                option.DefaultApiVersion = new ApiVersion(1, 0);
                //请求版本号放到Header中，注释掉则在url中指定
                //option.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //版本号放到URL中
                option.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'V";
                //option.AssumeDefaultVersionWhenUnspecified = true;
            });

            this.provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();


            //注册Swagger生成器，定义一个和多个Swagger 文档
            //属性>生成>XML文档文件 bin\debug\netcoreapp2.2\Swagger.xml
            //属性>生成>禁止显示警告 1701;1702
            services.AddSwaggerGen(c =>
            {
                // 多版本控制
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    // 添加文档信息
                    c.SwaggerDoc(item.GroupName, new Info
                    {
                        Title = "接口文档",
                        Version = item.ApiVersion.ToString(),
                        Description = "站点API文档"
                    });
                }
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = System.IO.Path.Combine(basePath, "Swagger.xml");
                c.IncludeXmlComments(xmlPath, true);
            });

            #endregion

            #region MVC

            //添加缓存支持
            services.AddMemoryCache();

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

                //图片不存在时返回默认图片
                app.UseDefaultImage(defaultImagePath: Configuration.GetSection("defaultImagePath").Value);

                app.UseHsts();
            }

            #endregion

            #region 其他公共

            //处理404请求
            app.UseRequestNotFound();

            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new CustomTraceListener());

            #endregion

            #region Hangfire计划任务

            ////Hangfire计划任务
            //app.UseHangfireServer();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });

            #endregion

            #region Swagger API文档生成

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniversalAPP Web API Document"); 单版本
                    c.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", "API V" + item.ApiVersion);
                }
            });

            #endregion

            #region MVC相关

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            //获取IP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //为了在nginx上获取用户端真实IP添加此代码
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //app.UseCookiePolicy();//增加对于GDPR政策的支持
            app.UseSession();
            //拓展MIME支持
            var unknownFileOption = new FileExtensionContentTypeProvider();
            unknownFileOption.Mappings[".myapp"] = "application/x-msdownload";
            unknownFileOption.Mappings[".htm3"] = "text/html";
            unknownFileOption.Mappings[".image"] = "image/png";
            unknownFileOption.Mappings[".rtf"] = "application/x-msdownload";
            unknownFileOption.Mappings[".apk"] = "application/vnd.android.package-archive";
            unknownFileOption.Mappings[".rar"] = "application/octet-stream";
            ////移除mp4支持
            //unknownFileOption.Mappings.Remove(".mp4");
            app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = unknownFileOption });

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


        }
    }
}
