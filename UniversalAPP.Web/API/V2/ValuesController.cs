using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.API.V2
{
    /// <summary>
    /// V2版本演示
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class valuesController : BaseAPIController
    {

        private Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appkeys"></param>
        public valuesController(IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _config_basic = appkeys.Value;

        }

        /// <summary>
        /// V2版本测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            System.Diagnostics.Trace.Write("Hass");
            return ResultBasicString(1, "OK", "V2版本接口已通");
        }

    }
}