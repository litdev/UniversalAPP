﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniversalAPP.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Hosting;

namespace UniversalAPP.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Models.SiteConfig _config;
        private readonly EFCore.EFDBContext _context;
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment environment,ILoggerFactory loggerFactory, IOptionsSnapshot<SiteConfig> appkeys,EFCore.EFDBContext context)
        {
            _env = environment;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _config = appkeys.Value;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public static IConfigurationRoot Configuration { get; set; }
        
        public IActionResult SendEmailTest()
        {
            Tools.EmailHelper emailHelper = new Tools.EmailHelper();
            NameValueCollection myCol = new NameValueCollection();
            myCol.Add("ename", "litdev");
            myCol.Add("link", "http://www.google.com");
            string templatePath = "~/wwwroot/mailtemplate/demo.html";
            var status = emailHelper.Send(_env.ContentRootPath, templatePath, "测试邮件--", "litdev@outlook.com", myCol);
            return Content($"发送状态：{status}");
        }

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