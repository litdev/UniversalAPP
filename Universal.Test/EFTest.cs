using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UniversalAPP.Test
{
    [TestClass]
    public class EFTest
    {
        [TestMethod]
        public async Task Test()
        {
            string connectionString = "Data Source=120.25.216.194,3763;database=dbcore;uid=expdemo;pwd=exp1234;";
            var context = new EFCore.EFDBContext(connectionString);

            //BLL.DynamicBLL<Entity.SysUser> bll = new BLL.DynamicBLL<Entity.SysUser>(context);
            //for (int i = 1; i <= 30; i++)
            //{
            //    var user = new Entity.SysUser()
            //    {
            //        NickName = "测试数据-"+i.ToString(),
            //        Password = "YlTfieAJPbSUwvJkC/qmdQ==",
            //        Status = true,
            //        SysRoleID = 1,
            //        UserName = "test"+i.ToString(),
            //        Gender = Entity.UserGender.男,
            //        Avatar = ""
            //    };
            //    bll.Add(user);
            //}
            //string where = "SysRoleID == 1 && (NickName.Contains(@0) || UserName.Contains(@0))";
            //var result = bll.GetPagedList(1, 2, where, "RegTime DESC", "SysRole","1");


            //var ss = bll.DelByIds("100,102,103");

            BLL.BLLCusCategory bll = new BLL.BLLCusCategory(context);
            //var ss = bll.GetChildIDStr(1);
            var ss = await bll.GetList(true, 6);
            var sss = await bll.GetList(false, 1);

            Assert.AreEqual(1,1);
        }
    }
}
