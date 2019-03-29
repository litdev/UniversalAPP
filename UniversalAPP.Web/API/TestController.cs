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
    /// <summary>
    /// 测试控制器
    /// </summary>
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : BaseAPIController
    {
        /// <summary>
        /// 测试接口是否通
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            //throw new Exception("sss");
            return ResultBasicString(1, "OK", "接口已通");
        }

        /// <summary>
        /// 带参数的接口
        /// </summary>
        /// <param name="val">随便写点啥</param>
        /// <returns></returns>
        [HttpGet]
        [Route("doing/other")]
        public UnifiedResultEntity<int> doing2(string val)
        {
            UnifiedResultEntity<int> resultEntity = new UnifiedResultEntity<int>();
            resultEntity.msg = 1;
            resultEntity.msgbox = $"接口已通:{val}";
            resultEntity.data = 222;
            return resultEntity;
        }

    }
}