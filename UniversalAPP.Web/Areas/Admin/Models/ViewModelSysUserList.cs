using System.Collections.Generic;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    public class ViewModelSysUserList : BasePageModel
    {
        /// <summary>
        /// 当前筛选的组
        /// </summary>
        public int role { get; set; }

        /// <summary>
        /// 当前筛选的关键字
        /// </summary>
        public string word { get; set; }

        /// <summary>
        /// 用户列表
        /// </summary>
        public List<Entity.SysUser> DataList { get; set; }


    }
}
