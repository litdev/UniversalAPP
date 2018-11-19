using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalAPP.Web.Models
{
    public class SiteConfig
    {
        #region 站点综合设置

        /// <summary>
        /// 站点URL
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// 默认后台默认分页每页数量
        /// </summary>
        public int AdminDefaultPageSize { get; set; }

        /// <summary>
        /// 页面耗时统计
        /// </summary>
        public bool WebExecutionTime { get; set; }

        /// <summary>
        /// 操作日志是否入库
        /// </summary>
        public bool LogMethodInDB { get; set; }

        /// <summary>
        /// 异常日志是否入库
        /// </summary>
        public bool LogExceptionInDB { get; set; }

        #endregion

        #region 接口配置

        /// <summary>
        /// 接口是否开启安全验证
        /// </summary>
        public bool WebAPIAuthentication { get; set; }

        /// <summary>
        /// 接口Token的键名
        /// </summary>
        public string WebAPITokenKey { get; set; }

        /// <summary>
        /// 接口Token混淆字符串
        /// </summary>
        public string WebAPIMixer { get; set; }

        /// <summary>
        /// 接口超时时间（分钟）
        /// </summary>
        public int WebAPITmeOutMinute { get; set; }


        #endregion

    }
}
