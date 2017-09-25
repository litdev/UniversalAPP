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
    public class TestController : Controller
    {
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            //throw new Exception("sss");
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
            result.msg = 1;
            result.msgbox = "ok";
            result.data = "接口已通";
            return result;
        }
    }
}