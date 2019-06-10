using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIDSSO
{
    using Common;
    using Infrastructure;
    using Models;
    [TestClass]
    public class RedisHelperTest
    {

        private UserModel TestUser
        {
            get
            {
                return new UserModel
                {
                    ID = 1,
                    UserName = "zhangsan",
                    Email = "123@qq.com",
                    Gender = 'F',
                    Birthday = DateTime.Now,
                    RegDate = DateTime.Now,
                    Password = "123456"
                };
            }
        }
        [TestMethod,TestCategory(" set user info")]
        public void SetUserInfoTest()
        {

            var user = TestUser;
            RedisHelper.SetUserInfo(user.UserName, user);
            var tmpUser = RedisHelper.GetUserInfo(user.UserName);
            Assert.IsNotNull(tmpUser);
            Assert.AreEqual(user.UserName, tmpUser.UserName);

        }

        [TestMethod,TestCategory("user info")]
        public void EmptyIsOk()
        {
            Assert.IsNull(RedisHelper.GetUserInfo("张三"));
        }
        [TestMethod,TestCategory("user info")]
        public void ClearUserInfo()
        {
            RedisHelper.SetUserInfo(TestUser.UserName, TestUser);
            RedisHelper.ClearUserInfo(TestUser.UserName);
            Assert.IsNull(RedisHelper.GetUserInfo(TestUser.UserName));
        }
    }
}
