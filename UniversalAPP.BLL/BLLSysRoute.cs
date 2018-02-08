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
    public class BLLSysRoute:BaseBLL
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
    }
}
