using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Users.Proxy;
using Flow.Users.Contract.Message;

namespace UsersService
{
    [TestClass]
    public class UsersService
    {
        [TestMethod]
        public void Can_Call_Proxy_For_User()
        {
            using (FlowUsersService proxy = new FlowUsersService("FlowUsersService_Endpoint"))
            {
                proxy.IsValidUser(new IsValidUserRequest { Domain = "google", User = "cgrant" });
            }
        }
    }
}
