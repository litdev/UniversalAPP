using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UniversalAPP.EFCore;

namespace UniversalAPP.EFCoreMigrator
{
    /// <summary>
    /// EF Core自动迁移小工具，放到服务器上执行
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Entity Framework Core Migrate Start !");
            Console.WriteLine("Get Pending Migrations...");

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UniversalCoreAPPDB;Trusted_Connection=True;MultipleActiveResultSets=true;";
            using (var db = new EFDBContext(connectionString))
            {
                //获取所有迁移
                Console.WriteLine($"Pending Migrations：\n{string.Join('\n', db.Database.GetPendingMigrations().ToArray())}");

                Console.WriteLine("Do you want to continue?(Y/N)");

                if (Console.ReadLine().Trim().ToLower() == "n")
                {
                    return;
                }

                Console.WriteLine("Migrating...");

                try
                {

                    //执行迁移
                    db.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine("Entity Framework Core Migrate Complete !");
                Console.WriteLine("Press any key for exit !");
                Console.ReadKey();
            }

        }
    }
}
