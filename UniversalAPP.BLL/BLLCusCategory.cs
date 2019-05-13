using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace UniversalAPP.BLL
{
    /// <summary>
    /// 无限级分类演示
    /// </summary>
    public class BLLCusCategory : BaseBLL
    {
        public BLLCusCategory(EFCore.EFDBContext context) { this.db = context; }

        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public Task<int> Del(int id)
        //{
        //    if (id <= 0) return Task.Factory.StartNew(() => { return -1; });
        //    var ids = GetChildIDStr(id);
        //    if (string.IsNullOrWhiteSpace(ids)) return Task.Factory.StartNew(() => { return -1; });
        //    var tableName = db.Model.FindEntityType(typeof(Entity.CusCategory).FullName).Relational().TableName;
        //    var del_sql = $"Delete {tableName} Where ID in({ids})";
        //    return db.Database.ExecuteSqlCommandAsync(del_sql);
        //}

        /// <summary>
        /// 添加分类数据
        /// </summary>
        /// <returns></returns>
        public Task<int> Add(Entity.CusCategory entity)
        {
            if (entity == null)
                return Task.Factory.StartNew(() => { return -1; });

            Entity.CusCategory p_entity = null;
            if (entity.PID != null)
            {
                p_entity = db.CusCategorys.Find(entity.PID);
                if (p_entity == null)
                {
                    entity.PID = null;
                    entity.Depth = 1;
                }
                else
                {
                    entity.Depth = p_entity.Depth + 1;
                }
            }

            db.CusCategorys.Add(entity);
            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 修改分类数据
        /// </summary>
        /// <returns></returns>
        public Task<int> Modify(Entity.CusCategory entity)
        {
            if (entity == null)
                return Task.Factory.StartNew(() => { return -1; });

            Entity.CusCategory p_entity = null;
            if (entity.PID != null)
            {
                p_entity = db.CusCategorys.Find(entity.PID);
                if (p_entity == null)
                {
                    entity.PID = null;
                    entity.Depth = 1;
                }
                else
                {
                    entity.Depth = p_entity.Depth + 1;
                }
            }

            db.Entry(entity).State = EntityState.Modified;
            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 根据ID获取子或父数据
        /// </summary>
        /// <param name="up">查找父级，否则为查找子级</param>
        /// <param name="id">当前分类ID</param>
        /// <returns></returns>
        public List<Entity.CusCategory> GetList(bool up, int id)
        {
            var exc_sql = up ? "SELECT* FROM CusCategorys WHERE FIND_IN_SET(ID, fn_queryParentCusCategory(" + id.ToString() + "));" : "SELECT* FROM CusCategorys WHERE FIND_IN_SET(ID, fn_queryChildCusCategory(" + id.ToString() + "));";
            return db.Database.SqlQuery<Entity.CusCategory>(exc_sql);
        }

        /// <summary>
        /// 获取某个分类下所有的子类ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetChildIDStr(int id)
        {
            string Sql = $"select fn_queryChildCusCategory({id}) as idstr";

            var str = db.Database.SqlQuery<CusCategoryChildIDStr>(Sql);
            return str.FirstOrDefault()?.idstr.Replace("$,", "");
            //string result = string.Empty;
            //using (var connection = db.Database.GetDbConnection())
            //{
            //    connection.Open();
            //    using (var command = connection.CreateCommand())
            //    {
            //        command.CommandText = Sql;
            //        using (SqlDataReader reader = command.ExecuteReader() as SqlDataReader)
            //        {
            //            while (reader.Read()) result = reader["idstr"].ToString();
            //        }
            //    }
            //}
            //return result;
        }


    }

    internal class CusCategoryChildIDStr
    {
        public string idstr { get; set; }
    }


}
