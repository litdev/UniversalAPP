using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace UniversalAPP.Web
{
    
    public class SimpleResourceFilterAttribute : Attribute, IResourceFilter
    {
        private readonly ILogger<SimpleResourceFilterAttribute> _logger;

        public SimpleResourceFilterAttribute(ILogger<SimpleResourceFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _logger.LogInformation("ResourceFilter Executed!");
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _logger.LogInformation("ResourceFilter Executing!");
        }

    }
}
