using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace UniversalAPP.Web
{
    public class SimpleResultFilterAttribute : Attribute, IResultFilter
    {
        private readonly ILogger<SimpleResultFilterAttribute> _logger;

        public SimpleResultFilterAttribute(ILogger<SimpleResultFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("ResultFilter Executd!");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            _logger.LogInformation("ResultFilter Executing!");
        }
    }

}
