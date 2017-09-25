using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace UniversalAPP.Tools
{
    public class AESHelper
    {
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="toEncrypt">要加密的字符串</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(toEncrypt) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(iv)) return string.Empty;
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="toDecrypt">要解密的字符串</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(toDecrypt) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(iv)) return string.Empty;
            try
            {

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.BlockSize = 128;
                rDel.KeySize = 256;
                rDel.FeedbackSize = 128;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("AES解密失败：" + ex.Message);
                return string.Empty;
            }
        }

    }
}
