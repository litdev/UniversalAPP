using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.API.V1
{
    /// <summary>
    /// 测试控制器V1
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
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
        /// 异常接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("error")]
        public UnifiedResultEntity<string> error()
        {
            throw new Exception("手动抛出异常");
        }

        /// <summary>
        /// 测试接口是否通
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            return ResultBasicString(1, "OK", "V1接口已通");
        }

        /// <summary>
        /// 带参数的接口
        /// </summary>
        /// <param name="val">随便写点啥</param>
        /// <returns></returns>
        [HttpGet]
        [Route("doing2")]
        public UnifiedResultEntity<int> doing2(string val)
        {
            UnifiedResultEntity<int> resultEntity = new UnifiedResultEntity<int>();
            resultEntity.msg = 1;
            resultEntity.msgbox = $"接口已通:{val}";
            resultEntity.data = 222;
            return resultEntity;
        }

        /// <summary>
        /// 隐藏API，增加这个属性[ApiExplorerSettings(IgnoreApi = true)]
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("hidden")]
        [ApiExplorerSettings(IgnoreApi =true)]
        public UnifiedResultEntity<string> HiddenAPI()
        {
            return ResultBasicString(1, "Hidden", "此接口已隐藏");
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="files">客户端上传文件的KEY必须为“files”，多个文件各个名字都写“files”，不然取不到文件</param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploads")]
        public UnifiedResultEntity<List<string>> Uploads(IList<IFormFile> files)
        {
            UnifiedResultEntity<List<string>> result_entity = new UnifiedResultEntity<List<string>>();
            if (files.Count == 0)
            {
                result_entity.msgbox = "请上传文件";
                return result_entity;
            }
            List<string> list_server_files = new List<string>();
            string save_path = "/files/api/";
            UploadHelper uploadHelper = new UploadHelper(_env);
            foreach (var item in files)
            {
                var temp_up = uploadHelper.Upload(item, save_path);
                if (temp_up.msg == 1) list_server_files.Add(temp_up.data);
            }

            result_entity.msg = 1;
            result_entity.msgbox = $"成功上传{list_server_files.Count}个文件";
            result_entity.data = list_server_files;
            return result_entity;
        }


    }
}