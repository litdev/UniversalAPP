using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalAPP.Web
{
    public class BaseAPIController : ControllerBase
    {

        /// <summary>
        /// 基础API返回String类型
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgbox"></param>
        /// <param name="data"></param>
        /// <param name="ext_int"></param>
        /// <param name="ext_str"></param>
        /// <returns></returns>
        protected UnifiedResultEntity<string> ResultBasicString(int msg, string msgbox, string data, int ext_int = 0, string ext_str = "")
        {
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
            result.msg = msg;
            result.msgbox = msgbox;
            result.data = data;
            result.ext_int = ext_int;
            result.ext_str = ext_str;
            return result;
        }

    }
}
