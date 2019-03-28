using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace UniversalAPP.Tools
{
    public class EmailHelper
    {
        private string _host = string.Empty;
        private int _port = 0;
        private bool _enableSsl = false;
        private string _mailFrom = string.Empty;
        private string _mailPwd = string.Empty;

        public EmailHelper()
        {
            _host = AppConfigurtaionServices.Configuration["Email:EmailHost"];
            _port = TypeHelper.ObjectToInt(AppConfigurtaionServices.Configuration["Email:EmailPort"]);
            _enableSsl = TypeHelper.ObjectToBool(AppConfigurtaionServices.Configuration["Email:EnableSsl"]);
            _mailFrom = AppConfigurtaionServices.Configuration["Email:From"];
            _mailPwd = AppConfigurtaionServices.Configuration["Email:Pwd"];
        }

        public bool Send(string title, string to_user, string content)
        {
            EmailToolHelper emailToolHelper = new EmailToolHelper(_host, _port, _enableSsl, _mailFrom, _mailPwd);
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
        public bool Send(string _ContentRootPath, string templateServerPath, string title, string to_user, NameValueCollection col)
        {
            FileHelper fileHelper = new FileHelper(_ContentRootPath);
            string templatePath = fileHelper.MapPath(templateServerPath);
            if (!fileHelper.IsExist(templatePath, false))
            {
                throw new Exception("邮件模板不存在");
            }

            string email_content = EmailTemplateHelper.BulidByFile(templatePath, col);

            EmailToolHelper emailToolHelper = new EmailToolHelper(_host, _port, _enableSsl, _mailFrom, _mailPwd);
            emailToolHelper.isbodyHtml = true;
            emailToolHelper.mailSubject = title;
            emailToolHelper.mailToArray = to_user.Split(',');
            emailToolHelper.mailBody = email_content;
            return emailToolHelper.Send();

        }
    }
}
