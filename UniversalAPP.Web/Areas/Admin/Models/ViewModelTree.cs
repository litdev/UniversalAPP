using System.Runtime.Serialization;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    public class ViewModelTree
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool open { get; set; }

        /// <summary>
        /// 指定序列化成员名称
        /// </summary>
        [DataMember(Name = "checked")]
        public bool is_checked { get; set; }
    }
}
