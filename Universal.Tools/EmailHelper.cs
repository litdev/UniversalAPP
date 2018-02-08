using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace UniversalAPP.Tools
{
    public class EmailHelper
    {
        public bool Send(string title, string to_user,string content)
        {
            EmailToolHelper emailToolHelper = new EmailToolHelper();
            emailToolHelper.enableSsl = TypeHelper.ObjectToBool(AppConfigurtaionServices.Configuration["SiteConfig:EmailEnableSsl"]);
            emailToolHelper.mailFrom = AppConfigurtaionServices.Configuration["SiteConfig:EmailFrom"];
            emailToolHelper.mailPwd = AppConfigurtaionServices.Configuration["SiteConfig:EmailPwd"];
            emailToolHelper.port = TypeHelper.ObjectToInt(AppConfigurtaionServices.Configuration["SiteConfig:EmailPort"]);
            emailToolHelper.host = AppConfigurtaionServices.Configuration["SiteConfig:EmailHost"];
            emailToolHelper.isbodyHtml = true;
            emailToolHelper.mailSubject = title;
            emailToolHelper.mailToArray = to_user.Split(',');
            emailToolHelper.mailBody = content;
            return emailToolHelper.Send();
        }


        /// <summary>
        /// 发送邮件，使用模板
        /// </summary>
        /// <param name="_ContentRootPath">环境根目录</param>
        /// <param name="templateServerPath">模板目录("~/wwwroot/mailtemplate/demo.html")</param>
        /// <param name="title"></param>
        /// <param name="to_user"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool Send(string _ContentRootPath,string templateServerPath, string title, string to_user, NameValueCollection col)
        {
            FileHelper fileHelper = new FileHelper(_ContentRootPath);
            string templatePath = fileHelper.MapPath(templateServerPath);
            if(!fileHelper.IsExist(templatePath, false))
            {
                throw new Exception("邮件模板不存在");
            }

            string email_content = EmailTemplateHelper.BulidByFile(templatePath, col);

            EmailToolHelper emailToolHelper = new EmailToolHelper();
            emailToolHelper.enableSsl = TypeHelper.ObjectToBool(AppConfigurtaionServices.Configuration["SiteConfig:EmailEnableSsl"]) ;
            emailToolHelper.mailFrom = AppConfigurtaionServices.Configuration["SiteConfig:EmailFrom"];
            emailToolHelper.mailPwd = AppConfigurtaionServices.Configuration["SiteConfig:EmailPwd"];
            emailToolHelper.port = TypeHelper.ObjectToInt(AppConfigurtaionServices.Configuration["SiteConfig:EmailPort"]);
            emailToolHelper.host = AppConfigurtaionServices.Configuration["SiteConfig:EmailHost"];
            emailToolHelper.isbodyHtml = true;
            emailToolHelper.mailSubject = title;
            emailToolHelper.mailToArray = to_user.Split(',');
            emailToolHelper.mailBody = email_content;
            return true;//emailToolHelper.Send();

        }
    }
}
