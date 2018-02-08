using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace UniversalAPP.BLL
{

    public class DynamicBLL<T> where T : class, new()
    {
        EFCore.EFDBContext db;

        public DynamicBLL(EFCore.EFDBContext context)
        {
            this.db = context;
        }


        #region 添加
        
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(T entity)
        {
            db.Set<T>().Add(entity);
            return db.SaveChanges();
        }

        /// <summary>
        /// 添加 Async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> AddAsync(T entity)
        {
            db.Set<T>().Add(entity);
            return db.SaveChangesAsync();
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public int Del(string where, params string[] whereArgs)
        {
            var result = db.Set<T>().Where(where,whereArgs).ToList();
            result.ForEach(p => db.Remove(p));
            return db.SaveChanges();
        }

        /// <summary>
        /// 删除 Async
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<int> DelAsync(string where, params string[] whereArgs)
        {
            var result = db.Set<T>().Where(where,whereArgs).ToList();
            result.ForEach(p => db.Remove(p));
            return db.SaveChangesAsync();
        }

        #endregion

        #region 查询数量

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public int GetCount(string where,params string[] whereArgs)
        {
            return db.Set<T>().Count(where,whereArgs);
        }

        /// <summary>
        /// 查询数量 Async
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<int> GetCountAsync(string where, params string[] whereArgs)
        {
            return db.Set<T>().CountAsync(where,whereArgs);
        }
        #endregion

        #region 是否存在

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public bool Exists(string where, params string[] whereArgs)
        {
            return db.Set<T>().Any(where,whereArgs);
        }

        /// <summary>
        /// 是否存在 Async
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string where, params string[] whereArgs)
        {
            return db.Set<T>().AnyAsync(where,whereArgs);
        }
        #endregion

        #region 根据条件查询单个model

        /// <summary>
        /// 根据条件查询单个model
        /// </summary>
        /// <param name="includePath"></param>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public T GetModel(string includePath, string orderby, string where, params string[] whereArgs)
        {
            var query = db.Set<T>().Where(where,whereArgs);
            foreach (var path in includePath.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    query = query.Include(path);
            }
            if (string.IsNullOrWhiteSpace(orderby))
                return query.AsNoTracking().FirstOrDefault();
            else
                return query.OrderBy(orderby).AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// 根据条件查询单个model Async
        /// </summary>
        /// <param name="includePath"></param>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<T> GetModelAsync(string includePath, string orderby, string where, params string[] whereArgs)
        {
            var query = db.Set<T>().Where(where,whereArgs);
            foreach (var path in includePath.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    query = query.Include(path);
            }
            if (string.IsNullOrWhiteSpace(orderby))
                return query.AsNoTracking().FirstOrDefaultAsync();
            else
                return query.OrderBy(orderby).AsNoTracking().FirstOrDefaultAsync();
        }


        #endregion

        #region 根据条件查询集合

        /// <summary>
        /// 根据条件查询集合
        /// </summary>
        /// <param name="top"></param>
        /// <param name="includePath"></param>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public List<T> GetList(int top, string includePath, string orderby, string where, params string[] whereArgs)
        {
            var query = db.Set<T>().Where(where,whereArgs).Take(top);
            foreach (var path in includePath.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    query = query.Include(path);
            }
            if (string.IsNullOrWhiteSpace(orderby))
                return query.AsNoTracking().ToList();
            else
                return query.OrderBy(orderby).AsNoTracking().ToList();
        }

        /// <summary>
        /// 根据条件查询集合 Async
        /// </summary>
        /// <param name="top"></param>
        /// <param name="includePath"></param>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<List<T>> GetListAsnyc(int top, string includePath, string orderby, string where, params string[] whereArgs)
        {
            var query = db.Set<T>().Where(where,whereArgs).Take(top);
            foreach (var path in includePath.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    query = query.Include(path);
            }
            if (string.IsNullOrWhiteSpace(orderby))
                return query.AsNoTracking().ToListAsync();
            else
                return query.OrderBy(orderby).AsNoTracking().ToListAsync();
        }

        #endregion

        #region 分页

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="page_index">当前第几页</param>
        /// <param name="page_size">每页大小</param>
        /// <param name="where">条件语句</param>
        /// <param name="orderby">排序</param>
        /// <param name="includePath">关联查询</param>
        /// <returns></returns>
        public Entity.PageEntity<T> GetPagedList(int page_index, int page_size, string includePath, string orderby, string where, params string[] whereArgs)
        {
            Entity.PageEntity<T> result = new Entity.PageEntity<T>();

            var query = db.Set<T>().Where(where, whereArgs);
            foreach (var path in includePath.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    query = query.Include(path);
            }
            var page_result = query.OrderBy(orderby).AsNoTracking().PageResult(page_index, page_size);
            var db_data = page_result.Queryable.ToList();
            result.data = db_data;
            result.page_count = page_result.PageCount;
            result.row_count = page_result.RowCount;
            return result;

        }

        #endregion

        #region 执行SQL语句，返回对象实体

        /// <summary>
        /// 执行SQL语句，返回对象实体
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public T GetModelFromSql(string orderby, string where, params string[] whereArgs)
        {
            return db.Set<T>().FromSql(where, whereArgs).OrderBy(orderby).FirstOrDefault();
        }

        /// <summary>
        /// 执行SQL语句，返回对象实体  Async
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<T> GetModelFromSqlAsync(string orderby, string where, params string[] whereArgs)
        {
            return db.Set<T>().FromSql(where, whereArgs).OrderBy(orderby).FirstOrDefaultAsync();
        }

        #endregion

        #region 执行SQL语句，返回对象集合

        /// <summary>
        /// 执行SQL语句，返回对象集合
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public List<T> GetListFromSql(string orderby, string where, params string[] whereArgs)
        {
            return db.Set<T>().FromSql(where, whereArgs).OrderBy(orderby).ToList();
        }

        /// <summary>
        /// 执行SQL语句，返回对象集合  Async
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="where"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public Task<List<T>> GetListFromSqlAsync(string orderby, string where, params string[] whereArgs)
        {
            return db.Set<T>().FromSql(where, whereArgs).OrderBy(orderby).ToListAsync();
        }

        #endregion

        #region 执行SQL语句，返回受影响的行数

        /// <summary>
        /// 执行SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlArgs"></param>
        /// <returns></returns>
        public int ExecuteSqlCommand(string sql, params string[] sqlArgs)
        {
            return db.Database.ExecuteSqlCommand(sql, sqlArgs);
        }

        /// <summary>
        /// 执行SQL语句，返回受影响的行数 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlArgs"></param>
        /// <returns></returns>
        public Task<int> ExecuteSqlCommandAsync(string sql,params string [] sqlArgs)
        {
            return db.Database.ExecuteSqlCommandAsync(sql, sqlArgs);
        }

        #endregion

        #region 执行SQL语句，根据ID批量删除数据，返回受影响的行数

        /// <summary>
        /// 执行SQL语句，根据ID批量删除数据，返回受影响的行数
        /// </summary>
        /// <param name="ids">例如：1,2,3,4</param>
        /// <returns></returns>
        public int DelByIds(string ids)
        {
            var tableName = db.Model.FindEntityType(typeof(T).FullName).Relational().TableName;
            var delSql = $"delete {tableName} where ID in({ids})";
            return db.Database.ExecuteSqlCommand(delSql);
        }

        /// <summary>
        /// 执行SQL语句，根据ID批量删除数据，返回受影响的行数 Async
        /// </summary>
        /// <param name="ids">例如：1,2,3,4</param>
        /// <returns></returns>
        public Task<int> DelByIdsAsync(string ids)
        {
            var tableName = db.Model.FindEntityType(typeof(T).FullName).Relational().TableName;
            var delSql = $"delete {tableName} where ID in({ids})";
            return db.Database.ExecuteSqlCommandAsync(delSql);
        }

        #endregion

    }

}
