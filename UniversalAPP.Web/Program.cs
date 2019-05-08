using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UniversalAPP.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            var host = BuildWebHost(args);

            //数据库初始化
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<EFCore.EFDBContext>();
                //数据库如果不存在则进行初始化
                if (!(context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) EFCore.DbInitializer.Initialize(context);
                if (context.Database.GetPendingMigrations().ToArray().Length > 0)
                {
                    var msg = "数据库有迁移，请先迁移完再启动站点";
                    logger.Error(msg);
                    NLog.LogManager.Shutdown();
                    host.Dispose();
                    throw new Exception(msg);
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .Build();
    }
}
