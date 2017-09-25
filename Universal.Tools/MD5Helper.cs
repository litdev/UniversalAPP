using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UniversalAPP.Tools
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5Helper
    {
        /// <summary>
        /// MD5散列
        /// </summary>
        public static string MD5(string inputStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashByte = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            StringBuilder sb = new StringBuilder();
            foreach (byte item in hashByte)
                sb.Append(item.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }
    }
}
