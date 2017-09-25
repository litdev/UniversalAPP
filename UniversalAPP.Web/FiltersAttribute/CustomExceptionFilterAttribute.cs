using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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



        public CustomExceptionFilterAttribute(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _logger = loggerFactory.CreateLogger<CustomExceptionFilterAttribute>();
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError("Exception Execute! Message:" + context.Exception.Message);

            //开发环境不对异常做处理，抛出最详细的异常信息
            if(_hostingEnvironment.IsDevelopment()) return;

            //接口异常处理
            if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
                result.msgbox = "系统发生错误";
                result.ext_str = context.Exception.Message;

                context.Result = new ObjectResult(result);
                context.ExceptionHandled = true;

            }
            else if(context.HttpContext.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
            {
                //后台管理的异常

            }else
            {
                //前台页面的异常
                var result_view = new ViewResult { StatusCode = 400, ViewName = "Error" };
                result_view.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                result_view.ViewData.Add("Exception", context.Exception);

                context.Result = result_view;
                context.ExceptionHandled = true;
            }
        }
    }

}
