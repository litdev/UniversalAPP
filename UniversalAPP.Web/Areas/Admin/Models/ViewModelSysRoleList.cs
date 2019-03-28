using System.Collections.Generic;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    public class ViewModelSysRoleList:BasePageModel
    {
        /// <summary>
        /// 当前筛选的关键字
        /// </summary>
        public string word { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Entity.SysRole> DataList { get; set; }
    }
}
