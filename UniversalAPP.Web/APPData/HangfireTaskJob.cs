using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
 * Hangfire使用方法：
 * 
    HangfireTaskJob hangfireTask = new HangfireTaskJob(_db);
    //立即执行
    Hangfire.BackgroundJob.Enqueue(() => hangfireTask.AddMethodLog("a"));
    //延迟
    Hangfire.BackgroundJob.Schedule(() => hangfireTask.AddMethodLog("a"),TimeSpan.FromDays(1));
    //循环
    Hangfire.RecurringJob.AddOrUpdate(() => hangfireTask.AddMethodLog("a"),Hangfire.Cron.Daily);
    //连续(通过将多个后台任务链接在一起来定义复杂的工作流)
    var task_a = Hangfire.BackgroundJob.Enqueue(() => hangfireTask.AddMethodLog("a"));
    Hangfire.BackgroundJob.ContinueWith(task_a, () => hangfireTask.AddMethodLog("a"));
 * 
 **/


namespace UniversalAPP.Web
{
    /// <summary>
    /// 定时任务回调方法
    /// </summary>
    public class HangfireTaskJob
    {
        private EFCore.EFDBContext _db;

        public HangfireTaskJob(EFCore.EFDBContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 演示操作数据库
        /// HangfireTaskJob hangfireTask = new HangfireTaskJob(_db);
        /// Hangfire.BackgroundJob.Enqueue(() => hangfireTask.AddMethodLog("日志内容"));
        /// </summary>
        /// <param name="val"></param>
        public void AddMethodLog(string val)
        {
            var entity = new Entity.SysLogMethod();
            entity.Detail = $"Hangfire添加:{val}";
            entity.SysUserID = 1;
            entity.Type = Entity.SysLogMethodType.Add;
            BLL.DynamicBLL<Entity.SysLogMethod> bll = new BLL.DynamicBLL<Entity.SysLogMethod>(_db);
            var re= bll.Add(entity);
        }
    }
}
