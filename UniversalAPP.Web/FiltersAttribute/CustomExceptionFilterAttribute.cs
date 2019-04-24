using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 统一异常处理
    /// </summary>
    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private Models.SiteBasicConfig _config_basic;
        protected EFCore.EFDBContext _db_context;



        public CustomExceptionFilterAttribute(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider, ILogger<CustomExceptionFilterAttribute> logger, IOptionsSnapshot<Models.SiteBasicConfig> appkeys, EFCore.EFDBContext db)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _logger = logger;
            _config_basic = appkeys.Value;
            _db_context = db;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "发生异常");

            //开发环境不对异常做处理，抛出最详细的异常信息
            if (_hostingEnvironment.IsDevelopment()) return;

            //接口异常处理
            if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
                result.msgbox = "系统发生错误";
                result.ext_str = context.Exception.Message;

                context.Result = new ObjectResult(result);

            }
            else if (context.HttpContext.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
            {
                //后台管理的异常
                LogInDB(context.Exception);
            }
            else
            {
                //前台页面的异常
                var result_view = new ViewResult { StatusCode = 400, ViewName = "Error" };
                result_view.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                result_view.ViewData.Add("Exception", context.Exception);
                context.Result = result_view;
                LogInDB(context.Exception);
            }
            context.ExceptionHandled = true;
        }

        private void LogInDB(Exception ex)
        {
            if (!_config_basic.LogExceptionInDB) return;
            var entity = new Entity.SysLogException()
            {
                AddTime = DateTime.Now,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace
            };
            new BLL.DynamicBLL<Entity.SysLogException>(_db_context).Add(entity);
        }

    }

}
