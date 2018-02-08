using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniversalAPP.Tools
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }

        static AppConfigurtaionServices()
        {
            var baseDir = Directory.GetCurrentDirectory();        
            Configuration = new ConfigurationBuilder()
            .SetBasePath(baseDir)
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }
    }
}
