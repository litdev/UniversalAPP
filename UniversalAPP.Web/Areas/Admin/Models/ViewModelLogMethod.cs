using System.Collections.Generic;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    public class ViewModelLogMethod : BasePageModel
    {
        /// <summary>
        /// 当前筛选的关键字
        /// </summary>
        public string word { get; set; }

        /// <summary>
        /// 操作类别
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Entity.SysLogMethod> DataList { get; set; }
    }
}
