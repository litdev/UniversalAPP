using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;
using UniversalAPP.Tools;

namespace UniversalAPP.BLL
{
    public class BLLSysRoute : BaseBLL
    {
        public BLLSysRoute(EFCore.EFDBContext context)
        {
            this.db = context;
        }

        /// <summary>
        /// 根据标签获取路由分组数据
        /// </summary>
        /// <returns></returns>
        public Task<List<IGrouping<string, Entity.SysRoute>>> GetListGroupByTag()
        {
            return db.SysRoutes.GroupBy(p => p.Tag).ToListAsync();
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="RoleId">用户组ID</param>
        /// <param name="SuperRole">是否是超级管理员组</param>
        /// <param name="PageKey">路由</param>
        /// <param name="isPost">是否post请求</param>
        /// <returns></returns>
        public bool CheckAdminPower(int RoleId, bool SuperRole, string PageKey, bool isPost)
        {
            if (SuperRole) return true;
            if (string.IsNullOrWhiteSpace(PageKey)) return true;
            PageKey = PageKey.ToLower();
            var is_post = isPost ? 1 : 0;
            //var tableName = db.Model.FindEntityType(typeof(Entity.CusCategory).FullName).Relational().TableName;
            var check_sql = $"select top 1 ID from SysRoleRoutes where SysRoleID ={RoleId} and SysRouteID=(select ID from SysRoutes where Route='{PageKey}' and IsPost={is_post})";
            var db_result = db.Database.SqlQuery<CheckAdminPowerID>(check_sql).FirstOrDefault();
            return db_result != null;
        }

        internal class CheckAdminPowerID
        {
            public int ID { get; set; }
        }


    }
}
