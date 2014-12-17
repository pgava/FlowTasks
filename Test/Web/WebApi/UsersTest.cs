using Flow.Tasks.Web.Controllers.Api;
using Flow.Users.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebApi
{
    [TestClass]
    public class UsersTest
    {
        [TestMethod]
        public void SignInTest()
        {
            var userMock = new Mock<IFlowUsersService>();

            var sut = new UsersController(userMock.Object);
            sut.ControllerContext = MockUtil.GetMockedControllerContext();

        }
    }
}
