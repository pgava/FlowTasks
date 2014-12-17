using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Flow.Library;
using Flow.Users.Data;
using Flow.Users.Data.Infrastructure;
using Flow.Users.Data.DAL;
using System.Web;
using Flow.Users.Contract.Interface;
using System.Web.Http.Hosting;
using System.Web.Http;
using Flow.Users.Contract.Message;

namespace Flow.Tasks.Test
{
    [TestClass]
    public class WebApiTest
    {

        [TestInitialize]
        public void CreateContext()
        {
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Users_From_Domains()
        {
        }

    }
}
