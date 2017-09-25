using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalAPP.Web
{
    /// <summary>
    /// 全局站点KEY
    /// </summary>
    public class GlobalKeyConfig
    {
        /// <summary>
        /// AES加密KEY
        /// </summary>
        public static readonly string AESKey = "kfulufepd3glda4r";

        /// <summary>
        /// AES加密偏移量
        /// </summary>
        public static readonly string AESIV = "0392039203920300";


        /// <summary>
        /// 登陆错误次数
        /// </summary>
        public static readonly string Session_Login_Fail_Total = "SESSION_LOGIN_FAIL";

        /// <summary>
        /// 后台管理用户的实体
        /// </summary>
        public static readonly string Session_Admin_User_Info = "ADMIN_USER_INFO";

        /// <summary>
        /// 是否记住我
        /// </summary>
        public static readonly string Cookie_Is_Remeber = "isrem";

        /// <summary>
        /// 用户Id
        /// </summary>
        public static readonly string Cookie_Login_UserID = "uid";

        /// <summary>
        /// 用户密码
        /// </summary>
        public static readonly string Cookie_Login_UserPassword = "upwd";


    }
}
