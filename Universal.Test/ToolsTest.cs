using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalAPP.Tools;
using System;
namespace UniversalAPP.Test
{
    [TestClass]
    public class ToolsTest
    {
        [TestMethod]
        public void Test3Test()
        {
            string key = "kfulufepd3glda4r";
            string iv = "0392039203920300";

            //string val1 = AESHelper.Encrypt("abcdrew!13212132", key, iv);
            //string val2 = AESHelper.Decrypt("fadsfafadfsadf", key, iv);

            string api_token_val = "netcoretest!" + WebHelper.ConvertToTimeStamp(DateTime.Now);
            string api_token = AESHelper.Encrypt(api_token_val, key, iv);

            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void MD5Test()
        {
            var val = MD5Helper.MD5("sj2015");
            Assert.AreEqual("7a2653c5b27cdcfdfdd077a6abaef2f8", val);
        }

        [TestMethod]
        public void TimeTest()
        {
            //var val1 = WebHelper.DateStringFromNow(DateTime.Now.AddDays(1));
            //var val2 = WebHelper.ConvertByteToGB(100324);
            //WebHelper.DownPicFromUrl("https://imgsa.baidu.com/forum/pic/item/dab44aed2e738bd4355105ebab8b87d6267ff9fc.jpg", @"C:\test", DateTime.Now.ToFileTime() + ".jpg");
            //var val3= WebHelper.ConvertToTimeStamp(DateTime.Now);
            //var val4 = WebHelper.ConvertToDateTime(val3);

            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void SMSTest()
        {

            var ss = SMSHelper.SendSms("17688700150", "2244");

            Assert.AreEqual(1,1);
        }

    }
}
