using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using UniversalAPP.Web.Models;
using Microsoft.AspNetCore.Hosting;

namespace UniversalAPP.Web.Controllers
{
    public class MongoDBController : Controller
    {
        private readonly ILogger<MongoDBController> _logger;
        private Models.SiteBasicConfig _config;
        private readonly IHostingEnvironment _env;
        private readonly MongoDB.Services.DemoService _demoService;

        public MongoDBController(IHostingEnvironment environment, ILogger<MongoDBController> logger, IOptionsSnapshot<SiteBasicConfig> appkeys, MongoDB.Services.DemoService demoService)
        {
            _env = environment;
            _logger = logger;
            _config = appkeys.Value;
            _demoService = demoService;
        }

        public IActionResult Index()
        {
            return Content("MongoDB演示");
        }

        public ActionResult<List<MongoDB.Models.Demo>> Get()
        {
            return _demoService.Get();
        }

        /// <summary>
        /// 根据id获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<MongoDB.Models.Demo> Get(string id)
        {
            var Demo = _demoService.Get(id);
            if (Demo == null)
            {
                return Content("找不到数据");
            }
            return Demo;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        public ActionResult<MongoDB.Models.Demo> Create(string name,int age)
        {
            MongoDB.Models.Demo model = new MongoDB.Models.Demo();
            model.Name = name;
            model.Age = age;
            _demoService.Create(model);
            return model;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        public ActionResult<MongoDB.Models.Demo> Update(string id, string name, int age)
        {
            var model = _demoService.Get(id);
            if (model == null)
            {
                return Content("找不到数据");
            }
            model.Name = name;
            model.Age = age;
            _demoService.Update(id, model);
            return model;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(string id)
        {
            var model = _demoService.Get(id);

            if (model == null)
            {
                return Content("找不到数据");
            }
            _demoService.Remove(model.Id);
            return NoContent();
        }

    }
}
