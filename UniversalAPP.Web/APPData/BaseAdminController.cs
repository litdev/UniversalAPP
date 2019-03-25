using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web
{
    public class BaseAdminController : Controller
    {
        protected IHostingEnvironment _env;
        protected EFCore.EFDBContext _db_context;

        public BaseAdminController()
        {
            var hca = ServiceLocator.Instance.GetService<IHttpContextAccessor>();
            _env = hca.HttpContext.RequestServices.GetService<IHostingEnvironment>();
            _db_context = hca.HttpContext.RequestServices.GetService<EFCore.EFDBContext>();

        }


        /// <summary>
        /// 基础API返回String类型
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgbox"></param>
        /// <param name="data"></param>
        /// <param name="ext_int"></param>
        /// <param name="ext_str"></param>
        /// <returns></returns>
        protected JsonResult ResultBasicString(int msg, string msgbox, string data, int ext_int = 0, string ext_str = "")
        {
            UnifiedResultEntity<string> result = new UnifiedResultEntity<string>();
            result.msg = msg;
            result.msgbox = msgbox;
            result.data = data;
            result.ext_int = ext_int;
            result.ext_str = ext_str;
            return Json(result);
        }



    }
}
