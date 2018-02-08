using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;

namespace UniversalAPP.BLL
{
    public class BLLAppVersion
    {
        private EFCore.EFDBContext db;

        public BLLAppVersion(EFCore.EFDBContext context)
        {
            db = context;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> Save(Entity.AppVersion entity)
        {

            var model = db.AppVersons.Find(entity.ID);
            if(model == null)
            {
                db.AppVersons.Add(entity);
            }else
            {
                model.APPType = entity.APPType;
                model.Content = entity.Content;
                model.DownUrl = entity.DownUrl;
                model.LinkUrl = entity.LinkUrl;
                model.LogoImg = entity.LogoImg;
                model.MD5 = entity.MD5;
                model.Platforms = entity.Platforms;
                model.Size = entity.Size;
                model.Version = entity.Version;
            }

            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Entity.AppVersion> GetModel(int id)
        {
            return db.AppVersons.AsNoTracking().SingleOrDefaultAsync(p => p.ID == id);
        }

        public Task<int> Del(int id)
        {
            var entity = db.AppVersons.Find(id);
            if (entity != null)
                db.AppVersons.Remove(entity);

            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> Exists(int id)
        {
            var result = db.AppVersons.AsQueryable().AnyAsync("ID=@0", id);
            return result;
        }

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public Task<int> Count()
        {
            var result = db.AppVersons.AsQueryable().CountAsync("AppType=@0", 1);
            return result;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public Task<List<Entity.AppVersion>> GetList()
        {
            var result = db.AppVersons.Where("VersionCode > 0 And AppType = 1 And DynamicFunctions.Like(Content,\"%2%\") ").ToListAsync();
            
            return result;
        }

    }
}
