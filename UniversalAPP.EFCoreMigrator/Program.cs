using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UniversalAPP.EFCore;

namespace UniversalAPP.EFCoreMigrator
{
    /// <summary>
    /// EF Core自动迁移小工具，发布后放到服务器上执行 dotnet UniversalAPP.EFCoreMigrator.dll
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Entity Framework Core 数据库迁移开始 !");
            Console.WriteLine("获取待迁移项...");

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UniversalCoreAPPDB;Trusted_Connection=True;MultipleActiveResultSets=true;";
            using (var db = new EFDBContext(connectionString))
            {
                //获取所有迁移
                Console.WriteLine($"待迁移项：\n{string.Join('\n', db.Database.GetPendingMigrations().ToArray())}");

                Console.WriteLine("确认迁移？(Y/N)");

                if (Console.ReadLine().Trim().ToLower() == "n")
                {
                    return;
                }

                Console.WriteLine("迁移中...");

                try
                {

                    //执行迁移
                    db.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine("迁移失败！"+e.Message);
                }

                Console.WriteLine("Entity Framework Core 迁移完成 !");
                Console.WriteLine("按任意键退出程序！");
                Console.ReadKey();
            }

        }
    }
}
