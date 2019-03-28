using System.Collections.Generic;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    public class ViewModelAppVersion : BasePageModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int platform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Entity.AppVersion> DataList { get; set; }
    }
}
