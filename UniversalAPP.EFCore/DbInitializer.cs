using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UniversalAPP.EFCore
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(EFDBContext context)
        {
            context.Database.EnsureCreated();


            //添加用户组
            if (context.SysRoles.Count() == 0)
            {
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
            }

            //添加管理员
            if (context.SysUsers.Count() ==0)
            {
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
            }

            //添加无限级分类数据
            if(context.CusCategorys.Count()==0)
            {
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
            }

            #region 初始化存储过程
            //按照某一个Id查询它及它的所有子级成员存储过程
            string SQLGetChildCusCategory = @"
                    CREATE PROCEDURE [dbo].[sp_GetChildCusCategory] (@Id int)
                    AS
                    BEGIN
                    WITH Record AS(
                        SELECT
                        Id,
                        Title,
                        PID,
                        Depth,
                        Status,
                        SortNo,
                        AddTime
                    FROM
                        CusCategorys(NOLOCK)
                        WHERE Id=@Id
                        UNION ALL
                            SELECT
                        a.Id Id,
                        a.Title Title,
                        a.PID PID,
                        a.Depth Depth,
                        a.Status Status,
                        a.SortNo SortNo,
                        a.AddTime AddTime
                    FROM
                        CusCategorys(NOLOCK) a JOIN Record b
                        ON a.PID=b.Id
                    )
 
                    SELECT
                        Id,
                        Title,
                        PID,
                        Depth,
                        Status,
                        SortNo,
                        AddTime
                    FROM
                        Record
                        WHERE Status=1
                        ORDER BY SortNo DESC     
                    END";

            //按照某一个Id查询它及它的所有父级成员存储过程
            string SQLGetParentCusCategory = @"
                        CREATE PROCEDURE [dbo].[sp_GetParentCusCategory] (@Id int)
                        AS
                        BEGIN
                        WITH Record AS(
                            SELECT
                            Id,
                            Title,
                            PId,
                            Depth,
                            Status,
                            SortNo,
                            AddTime
                        FROM
                            CusCategorys(NOLOCK)
                            WHERE Id=@Id
                            UNION ALL
                            SELECT
                            a.Id Id,
                            a.Title Title,
                            a.PId PId,
                            a.Depth Depth,
                            a.Status Status,
                            a.SortNo SortNo,
                            a.AddTime AddTime
                        FROM
                            CusCategorys(NOLOCK) a JOIN Record b
                            ON a.Id=b.PId
                        )
 
                        SELECT
                            Id,
                            Title,
                            PId,
                            Depth,
                            Status,
                            SortNo,
                            AddTime
                        FROM
                            Record
                            WHERE Status=1
                            ORDER BY SortNo DESC
     
                        END";


            //获取所有子类的id，以逗号分割
            string SQLFunGetChildCusCategoryStr = @"
                        CREATE FUNCTION [dbo].[fn_GetChildCusCategoryStr] (@Id int) RETURNS varchar(1000) 
                        AS
                            BEGIN
                        declare @a VARCHAR(1000);
                        set @a='';
                            WITH Record AS(
                                SELECT
                                Id,
                                PID,
		                            Status
                            FROM
                                CusCategorys(NOLOCK)
                                WHERE Id=@Id
                                UNION ALL
                                    SELECT
				                        a.Id Id,
				                        a.PID PID,
				                        a.Status Status
                                    FROM
                                        CusCategorys(NOLOCK) a JOIN Record b
                                        ON a.PID=b.Id
                                    )
                        SELECT @a=isnull(@a+',','')+ltrim(Id) FROM Record  WHERE Status=1  
                        return SUBSTRING(@a, 2, len(@a))
                        END
                        ";



            //分割字符串，通用函数
            string SQLSplitString = @"
                CREATE function [dbo].[func_splitstring]
                (@str nvarchar(max),@split varchar(10))
                returns @t Table (c1 varchar(100))
                as
                begin
	                if charindex(@split,@str) = 0
	                begin
		                insert @t(c1) values(@str)
		                return
	                end

                    declare @i int
                    declare @s int
                    set @i=1
                    set @s=1
                    while(@i>0)
                    begin    
                        set @i=charindex(@split,@str,@s)
                        if(@i>0)
                        begin
                            insert @t(c1) values(substring(@str,@s,@i-@s))
                        end   
                        else begin
                            insert @t(c1) values(substring(@str,@s,len(@str)-@s+1))
                        end
                        set @s = @i + 1   
                    end
                    return
                end
                ";

            ////按照某一个Id查询它及它的所有父级成员存储过程
            //context.Database.ExecuteSqlCommand(SQLGetParentCusCategory);
            ////按照某一个Id查询它及它的所有子级成员存储过程
            //context.Database.ExecuteSqlCommand(SQLGetChildCusCategory);
            ////获取所有子类的id，以逗号分割
            //context.Database.ExecuteSqlCommand(SQLFunGetChildCusCategoryStr);
            ////分割字符串，通用函数
            //context.Database.ExecuteSqlCommand(SQLSplitString);


            #endregion
        }
    }
}
