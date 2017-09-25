using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniversalAPP.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace UniversalAPP.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Models.SiteConfig _config;

        public HomeController(ILoggerFactory loggerFactory, IOptionsSnapshot<SiteConfig> appkeys)
        {
            _logger = loggerFactory.CreateLogger<HomeController>();
            _config = appkeys.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        public static IConfigurationRoot Configuration { get; set; }
        

        public IActionResult About()
        {
            throw new Exception("手动异常");
            //ViewData["Message"] = "Your application description page.";
            //return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
