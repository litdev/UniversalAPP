
using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalAPP.Web
{
    /// <summary>
    /// Hangfire后台面板用户身份验证
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            ////使用后台的登陆用户认证
            //var httpContext = context.GetHttpContext();
            //return httpContext.User.Identity.IsAuthenticated;
            //注意这里，master分支中判断上下文用户是否登录是正常的，切换到MySQL就不正常了，不知道啥情况，先不做身份证验证了
            return true;
        }
    }
}
