using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace UniversalAPP.Web
{
    public class SimpleActionFilterAttribute : Attribute, IActionFilter
    {
        private readonly ILogger<SimpleActionFilterAttribute> logger;

        public SimpleActionFilterAttribute(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SimpleActionFilterAttribute>();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("ActionFilter Executed!");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("ActionFilter Executing!");
        }
    }

}
