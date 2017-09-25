﻿using System.ComponentModel.DataAnnotations;

namespace UniversalAPP.Entity
{
    /// <summary>
    /// 成员
    /// </summary>
    public class DemoDept
    {
        public int ID { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        public int DemoID { get; set; }

        [Display(Name = "标题"), MaxLength(255, ErrorMessage = "最大长度255"), Required]
        public string Title { get; set; }

        [Display(Name = "图片"), MaxLength(255), Required]
        public string ImgUrl { get; set; }

        [Display(Name = "其他数据"), Required]
        public int Num { get; set; }

    }
}
