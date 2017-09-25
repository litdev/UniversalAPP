﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversalAPP.Entity
{
    /// <summary>
    /// 无限级分类
    /// </summary>
    public class CusCategory
    {
        public CusCategory()
        {
            this.Status = true;
            this.Depth = 1;
            this.AddTime = DateTime.Now;
            this.Weight = 0;
        }

        public int ID { get; set; }

        [Display(Name = "分类名称"), StringLength(30), Required(ErrorMessage = "分类名称不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int? PID { get; set; }

        /// <summary>
        /// 父级部门信息
        /// </summary>
        [ForeignKey("PID"), Display(Name = "父类")]
        public CusCategory PCategory { get; set; }

        /// <summary>
        /// 深度，从1递增
        /// </summary>
        public int Depth { get; set; }


        [Display(Name = "状态")]
        public bool Status { get; set; }

        /// <summary>
        /// 优先级，越大，同级显示的时候越靠前
        /// </summary>
        [Display(Name = "权重")]
        public int Weight { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
