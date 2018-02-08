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
    public class BLLDemo:BaseBLL
    {
        public BLLDemo(EFCore.EFDBContext context) { this.db = context; }

        /// <summary>
        /// 得到一个实体，多个incloud
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Entity.Demo> GetModel(int id)
        {
            if (id <= 0)
                return null;

            return db.Demos.Where(p => p.ID == id).Include(p => p.LastUpdateUser).Include(p => p.AddUser).Include(p => p.Albums).Include(p => p.Depts).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<int> Modify(Entity.Demo entity)
        {
            if (entity == null)
                return Task.Factory.StartNew(() => { return -1; });
            if (entity.ID == 0)
                return Task.Factory.StartNew(() => { return -1; });

            //删除旧数据
            //db.DemoAlbums.Where(p => p.DemoID == entity.ID).ToList().ForEach(p => db.Entry(p).State = EntityState.Deleted);
            //db.DemoDepts.Where(p => p.DemoID == entity.ID).ToList().ForEach(p => db.Entry(p).State = EntityState.Deleted);
            db.DemoAlbums.Where(p => p.DemoID == entity.ID).ToList().ForEach(p=>db.DemoAlbums.Remove(p));
            db.DemoDepts.Where(p => p.DemoID == entity.ID).ToList().ForEach(p => db.DemoDepts.Remove(p));

            var old_entity = db.Demos.Find(entity.ID);
            db.Entry(old_entity).CurrentValues.SetValues(entity);
            ((List<Entity.DemoAlbum>)entity.Albums).ForEach(p => db.Entry(p).State = EntityState.Added);
            foreach (var item in entity.Depts)
            {
                item.DemoID = entity.ID;
                db.Entry(item).State = EntityState.Added;
            }
            try
            {
                return db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("保存测试信息失败", ex);
            }
        }
    }
}
