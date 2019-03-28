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
    /// <summary>
    /// 用户组操作
    /// </summary>
    public class BLLSysRole : BaseBLL
    {
        public BLLSysRole(EFCore.EFDBContext context)
        {
            this.db = context;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name=""></param>
        /// <param name="qx">权限数据</param>
        /// <returns></returns>
        public Task<int> Add(Entity.SysRole entity, string qx)
        {
            db.SysRoles.Add(entity);
            if (!string.IsNullOrWhiteSpace(qx))
            {
                foreach (var item in qx.Split(','))
                {
                    int route_id = TypeHelper.ObjectToInt(item);
                    db.SysRoleRoutes.Add(
                        new Entity.SysRoleRoute() { SysRole = entity, SysRouteID = route_id }
                    );
                }
            }
            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="qx"></param>
        /// <returns></returns>
        public Task<int> Modify(Entity.SysRole entity, string qx)
        {
            if (entity == null)
                return Task.Factory.StartNew(() => { return 0; });
            if (entity.ID == 0)
                return Task.Factory.StartNew(() => { return 0; });

            var old_entity = db.SysRoles.Find(entity.ID);
            db.Entry(old_entity).CurrentValues.SetValues(entity);

            //修改权限数据
            if (string.IsNullOrWhiteSpace(qx))
            {
                db.SysRoleRoutes.Where(p => p.SysRoleID == entity.ID).ToList().ForEach(p => db.SysRoleRoutes.Remove(p));
            }
            else
            {
                List<int> new_id_list = qx.Split(',').Select(Int32.Parse).ToList();
                var route_list = db.SysRoleRoutes.Where(p => p.SysRoleID == entity.ID).ToList();
                List<int> route_id_list = new List<int>();
                foreach (var item in route_list)
                    route_id_list.Add(item.SysRouteID);
                //判断存在的差
                var route_del_list = route_id_list.Except(new_id_list).ToList();
                foreach (var item in route_del_list)
                {
                    //删除
                    var del_entity = db.SysRoleRoutes.Where(p => p.SysRouteID == item && p.SysRoleID == entity.ID).FirstOrDefault();
                    db.SysRoleRoutes.Remove(del_entity);

                }

                var route_add_list = new_id_list.Except(route_id_list).ToList();
                foreach (var item in route_add_list)
                {
                    //做增加
                    db.SysRoleRoutes.Add(new Entity.SysRoleRoute() { SysRoleID = entity.ID, SysRouteID = item });
                }
            }

            try
            {
                return db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("修改组信息失败", ex);
            }
        }


    }
}
