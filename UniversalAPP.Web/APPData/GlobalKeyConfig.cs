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
        /// 后台管理用户认证
        /// </summary>
        public static readonly string Admin_Authentication_Scheme = "Admin_Scheme";

        /// <summary>
        /// 后台分页默认每页大小
        /// </summary>
        public static readonly int Admin_Default_PageSize = 12;

        /// <summary>
        /// 后台Claim，是否是超级管理组KEY
        /// </summary>
        public static readonly string Admin_Claim_IsSuperAdminRole = "IsSuperAdminRole";

        /// <summary>
        /// 后台Claim，组ID KEY
        /// </summary>
        public static readonly string Admin_Claim_RoleID = "Admin_Claim_RoleID";

        /// <summary>
        /// 后台Claim，用户头像 KEY
        /// </summary>
        public static readonly string Admin_Claim_Avatar = "Avatar";

    }

    public enum AdminFileUploadType
    {
        /// <summary>
        /// 一张照片
        /// </summary>
        OnePicture,

        /// <summary>
        /// 多张照片
        /// </summary>
        MorePicture,

        /// <summary>
        /// 安卓安装包
        /// </summary>
        APK,

        /// <summary>
        /// IOS安装包
        /// </summary>
        IPA,

        /// <summary>
        /// 附件压缩包
        /// </summary>
        ZIP,

        /// <summary>
        /// FroalaEditor 富文本编辑器
        /// </summary>
        FroalaEditor
    }

}
