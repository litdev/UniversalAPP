using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace UniversalAPP.EFCore
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(EFDBContext context)
        {
            //if (!context.Database.EnsureCreated()) return;
            //第一次执行迁移
            context.Database.Migrate();

            //添加用户组
            var role_list = new List<Entity.SysRole>()
                {
                    new Entity.SysRole()
                    {
                        RoleName = "管理员",
                        RoleDesc="管理员组",
                        IsAdmin =true
                    },
                    new Entity.SysRole()
                    {
                        RoleName = "编辑用户",
                        RoleDesc = "编辑用户组",
                        IsAdmin = false
                    }
                };
            role_list.ForEach(p => context.SysRoles.Add(p));
            context.SaveChanges();


            //添加管理员
            var role_root = context.SysRoles.Where(p => p.RoleName == "管理员").FirstOrDefault();
            string pwd = Tools.AESHelper.Encrypt("admin", "kfulufepd3glda4r", "0392039203920300");
            var user_root = new Entity.SysUser()
            {
                NickName = "超级管理员",
                Password = pwd,
                Status = true,
                SysRole = role_root,
                UserName = "admin",
                Gender = Entity.UserGender.男,
                Avatar = ""
            };
            context.SysUsers.Add(user_root);
            context.SaveChanges();

            //添加无限级分类数据
            var category_a = new Entity.CusCategory();
            category_a.PID = null;
            category_a.Title = "国内";
            context.CusCategorys.Add(category_a);

            var category_b = new Entity.CusCategory();
            category_b.PID = null;
            category_b.Title = "世界";
            context.CusCategorys.Add(category_b);

            var category_1 = new Entity.CusCategory();
            category_1.PCategory = category_a;
            category_1.Title = "社会";
            category_1.Depth = 2;
            context.CusCategorys.Add(category_1);

            var category_2 = new Entity.CusCategory();
            category_2.PCategory = category_a;
            category_2.Title = "经济";
            category_2.Depth = 2;
            context.CusCategorys.Add(category_2);

            var category_3 = new Entity.CusCategory();
            category_3.PCategory = category_a;
            category_3.Title = "文化";
            category_3.Depth = 2;
            context.CusCategorys.Add(category_3);

            var category_4 = new Entity.CusCategory();
            category_4.PCategory = category_b;
            category_4.Title = "格局";
            category_4.Depth = 2;
            context.CusCategorys.Add(category_4);

            var category_5 = new Entity.CusCategory();
            category_5.PCategory = category_b;
            category_5.Title = "要闻";
            category_5.Depth = 2;
            context.CusCategorys.Add(category_5);

            var category_6 = new Entity.CusCategory();
            category_6.PCategory = category_b;
            category_6.Title = "趋势";
            category_6.Depth = 2;
            context.CusCategorys.Add(category_6);
            context.SaveChanges();


            #region 初始化存储过程

            //按照某一个Id查询它及它的所有子级成员函数
            string SQLGetChildCusCategory = @"
                    DROP FUNCTION IF EXISTS fn_queryChildCusCategory;
                    CREATE FUNCTION fn_queryChildCusCategory(rootId INT)
                    RETURNS VARCHAR(4000)
                    BEGIN
                    DECLARE sTemp VARCHAR(4000);
                    DECLARE sTempChd VARCHAR(4000);
                    
                    SET sTemp='$';
                    SET sTempChd = CAST(rootId AS CHAR);
                    
                    WHILE sTempChd IS NOT NULL DO
                    SET sTemp= CONCAT(sTemp,',',sTempChd);
                    SELECT GROUP_CONCAT(ID) INTO sTempChd FROM CusCategorys WHERE FIND_IN_SET(PID,sTempChd)>0;
                    END WHILE;
                    RETURN sTemp;    
                    END;";
            //使用方法 SELECT fn_queryChildCusCategory(1);  SELECT* FROM CusCategorys WHERE FIND_IN_SET(ID, fn_queryChildCusCategory(1));

            //按照某一个Id查询它及它的所有父级成员函数
            string SQLGetParentCusCategory = @"
                    DROP FUNCTION IF EXISTS fn_queryParentCusCategory;
                    CREATE FUNCTION fn_queryParentCusCategory(rootId INT)
                    RETURNS VARCHAR(4000)
                    BEGIN
                    DECLARE sTemp VARCHAR(4000);
                    DECLARE sTempChd VARCHAR(4000);

                    SET sTemp='$';
                    SET sTempChd = CAST(rootId AS CHAR);
                    SET sTemp = CONCAT(sTemp,',',sTempChd);

                    SELECT PID INTO sTempChd FROM CusCategorys WHERE ID = sTempChd;
                    WHILE sTempChd <> 0 DO
                    SET sTemp = CONCAT(sTemp,',',sTempChd);
                    SELECT PID INTO sTempChd FROM CusCategorys WHERE ID = sTempChd;
                    END WHILE;
                    RETURN sTemp;
                    END;";
            //使用方法  SELECT fn_queryParentCusCategory(3);  SELECT* FROM CusCategorys WHERE FIND_IN_SET(ID, fn_queryParentCusCategory(3));


            //按照某一个Id查询它及它的所有子级成员函数
            context.Database.ExecuteSqlCommand(SQLGetChildCusCategory);
            //按照某一个Id查询它及它的所有父级成员函数
            context.Database.ExecuteSqlCommand(SQLGetParentCusCategory);


            #endregion
        }
    }
}
