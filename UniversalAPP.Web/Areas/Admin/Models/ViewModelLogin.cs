﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalAPP.Web.Areas.Admin.Models
{
    /// <summary>
    /// 后台登陆表单实体类
    /// </summary>
    public class ViewModelLogin
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [RegularExpression(@"^[a-zA-Z0-9]{3,10}$", ErrorMessage = "用户名只能由3-10位字母或数字组成")]
        public string user_name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(10, ErrorMessage = "最大长度30")]
        public string password { get; set; }

        /// <summary>
        /// 是否记住登陆
        /// </summary>
        public bool is_rember { get; set; }
    }
}
