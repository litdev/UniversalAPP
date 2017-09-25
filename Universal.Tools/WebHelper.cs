using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UniversalAPP.Tools
{
    public class WebHelper
    {
        //随机数
        private static char[] constant =
      {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };

        /// <summary>
        /// 整形随机数
        /// </summary>
        private static int[] constantInt = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// 把时间改为几个月,几天前,几小时前,几分钟前,或几秒前
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if(span.TotalMilliseconds<0)
            {
                return "未来";
            }
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return
                    "1个月前";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return
                        "2周前";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return
                            "1周前";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return
                                            "1秒前";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 字节转换为GB，不足G的返回MB，保留小数点后1位
        /// </summary>
        /// <returns></returns>
        public static string ConvertByteToGB(long data)
        {
            if (data <= 0)
                return "0";

            string val = "0";
            float mb = data / 1024f / 1024f;
            if (mb < 1024)
                val = mb.ToString("F1") + "MB";
            else
                val = (mb / 1024f).ToString("F1") + "GB";

            return val;
        }

        /// <summary>
        /// 根据URL下载图片
        /// </summary>
        /// <param name="pic_web_url">图片的网络URL</param>
        /// <param name="folder_path">保存的绝对,结尾要有/符号</param>
        /// <param name="file_name">保存的文件名，包含后缀</param>
        public static void DownPicFromUrl(string pic_web_url, string folder_path, string file_name)
        {
            try
            {
                if(!folder_path.EndsWith("/")) folder_path = folder_path + "/";
                if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    wc.DownloadFile(pic_web_url, folder_path + file_name);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("WebHelper.DownPicFromUrl图片下载失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 日期转换为时间戳（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime time)
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(time.AddHours(-8) - Jan1st1970).TotalSeconds;
        }

        /// <summary>
        /// 时间戳转换为日期（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddSeconds(timeStamp).AddHours(8);
        }

        /// <summary>
        /// 计算时间差
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="action">取值类别,d=天，h=小时，m=分钟，s=秒，ad=总天数，ah=总小时，am=总分钟，as=总秒,ms=总毫秒</param>
        /// <returns></returns>
        public static double DateTimeDiff(DateTime starTime, DateTime endTime, string action)
        {
            TimeSpan ts = endTime - starTime;
            switch (action)
            {
                case "d"://天
                    return ts.Days;
                case "h"://小时
                    return ts.Hours;
                case "m": //分钟
                    return ts.Minutes;
                case "s"://秒
                    return ts.Seconds;
                case "ad"://总天数
                    return ts.TotalDays;
                case "ah"://总小时
                    return ts.TotalHours;
                case "am"://总分钟
                    return double.Parse(ts.TotalMinutes.ToString("F2"));
                case "as": //总秒
                    return ts.TotalSeconds;
                case "ms"://毫秒
                    return ts.TotalMilliseconds;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="Length">长度</param>
        /// <returns></returns>
        public static string GenerateRandomNumber(int Length)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

        /// <summary>
        /// 生成整形随机数
        /// </summary>
        /// <param name="Length">长度</param>
        /// <returns></returns>
        public static string GenerateRandomIntNumber(int Length)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constantInt[rd.Next(10)]);
            }
            return newRandom.ToString();
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }



    }
}
