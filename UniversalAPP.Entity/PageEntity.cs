using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalAPP.Entity
{
    /// <summary>
    /// 分页数据统一返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageEntity<T>
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int page_count { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int row_count { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> data { get; set; }

    }
}
