using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.API
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : BaseAPIController
    {
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            //throw new Exception("sss");
            return ResultBasicString(1, "OK", "接口已通");
        }
    }
}