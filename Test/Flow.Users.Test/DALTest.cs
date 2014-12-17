using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Users.Data;
using Flow.Library;
using Flow.Users.Data.Infrastructure;
using Flow.Users.Data.DAL;
using Flow.Library.EF;
using Flow.Users.Contract.Message;
using Flow.Library.Security;

namespace UsersFlowTest
{
    [TestClass]
    public sealed class DALTest : IDisposable
    {

        private FlowUsersEntities ctx;
        private DbContextAdapter adp;
        private FlowUsersUnitOfWork uow;
        private TestContext testContextInstance;

        #region IDisposable
        public void Dispose()
        {
            if (uow != null)
            {
                var disp = uow as FlowUsersUnitOfWork;
                disp.Dispose();
            }

            if (ctx != null)
            {
                ctx.Dispose();
            }

            if (adp != null)
            {
                adp.Dispose();
            }
        }
        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize]
        public void CreateContext()
        {
            ctx = new FlowUsersEntities();
            adp = new FlowUsersContextAdapter(ctx);
            uow = new FlowUsersUnitOfWork(adp);
        }

        [TestMethod]
        public void Should_Get_All_Users_For_List_Of_Roles()
        {
            var userManagement = new FlowUsersOperations(uow);

            var roles = new[] { "Dev", "Approver" };
            var res = userManagement.GetUsersByRoles(roles);

            Assert.AreEqual(7, res.Count());
            Assert.IsTrue(res.Contains("cgrant"));
            Assert.IsTrue(res.Contains("pnewman"));
            Assert.IsTrue(res.Contains("rredford"));
        }

        [TestMethod]
        public void Should_Check_If_Valid_User()
        {
            var userManagement = new FlowUsersOperations(uow);

            var domain = "acmestar";
            var res = userManagement.IsValidUser(domain, "CGRANT");

            Assert.IsTrue(res);
        }

        [TestMethod]
        public void Should_Check_If_Valid_User_Even_When_Domain_Not_Specified()
        {
            var userManagement = new FlowUsersOperations(uow);

            var domain = string.Empty;
            var res = userManagement.IsValidUser(domain, "CGRANT");

            Assert.IsTrue(res);
        }

        [TestMethod]
        public void Should_Get_All_Users_For_A_Domain()
        {
            var userManagement = new FlowUsersOperations(uow);

            var domain = "AcmeStar";
            var res = userManagement.GetUsersByDomains(new [] {domain});

            Assert.AreEqual(21, res.Count());
            Assert.IsTrue(res.Contains("cgrant"));
            Assert.IsTrue(res.Contains("pnewman"));
            Assert.IsTrue(res.Contains("hbogart"));
            Assert.IsTrue(res.Contains("knovak"));
            Assert.IsTrue(res.Contains("mmonroe"));
        }

        [TestMethod]
        public void Should_Get_All_Domains_For_A_User()
        {
            var userManagement = new FlowUsersOperations(uow);

            var user = "cgrant";
            var res = userManagement.GetDomainsForUser(user);

            Assert.AreEqual(1, res.Count());
            Assert.IsTrue(res.Contains("AcmeStar"));
        }

        [TestMethod]
        public void Should_Authenticate_User()
        {
            var userManagement = new FlowUsersOperations(uow);

            var res = userManagement.AuthenticateUser("cgrant", "pwd");

            Assert.IsTrue(res);

        }

        [TestMethod]
        public void Should_Encrypt_Password()
        {
            var encStr = Encryption.Encrypt("Admin#1234");

            Assert.AreEqual("Admin#1234", Encryption.Decrypt(encStr));
        }

        [TestMethod]
        public void Should_Get_All_Domains_And_Roles_For_User()
        {
            var userManagement = new FlowUsersOperations(uow);

            var res = userManagement.GetDomainRoleForUser("cgrant");

            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.Roles.Count());
            Assert.AreEqual(1, res.Domanis.Count());
        }

        [TestMethod]
        public void Should_Get_All_Users_Containing_C()
        {
            var userManagement = new FlowUsersOperations(uow);
            var res = userManagement.GetUserNames("c");

            Assert.IsNotNull(res);
            Assert.AreEqual(7, res.Count()); // cgrant, tcruise
        }

        [TestMethod]
        public void Should_Get_All_Roles_Containing_Dev()
        {
            var userManagement = new FlowUsersOperations(uow);
            var res = userManagement.GetRoles("dev");

            Assert.IsNotNull(res);
            Assert.AreEqual(2, res.Count());
            Assert.AreEqual(1, res.First().Users.Count());
            Assert.AreEqual("cgrant", res.First().Users.First());
        }

        [TestMethod]
        public void Should_Get_User_And_Following_Users()
        {
            var userManagement = new FlowUsersOperations(uow);
            var res = userManagement.GetUser("cgrant");

            Assert.IsNotNull(res);
            Assert.AreEqual("Cary", res.FirstName);
            Assert.AreEqual("Grant", res.LastName);
            Assert.AreEqual(3, res.Following.Count());

        }

        [TestMethod]
        public void Should_Be_Able_To_Add_Following_User()
        {
            var userManagement = new FlowUsersOperations(uow);
            userManagement.AddFollowingUser("cgrant", "rredford");
            var res = userManagement.GetUser("cgrant");

            Assert.IsNotNull(res);
            Assert.AreEqual(4, res.Following.Count());
            Assert.IsTrue(res.Following.Any(f => f.UserName == "rredford"));

            var todel = uow.UserFollowings.Find(uf => uf.FollowerUser.Name == "cgrant" && uf.FollowingUser.Name == "rredford");
            foreach (var d in todel)
            {
                uow.UserFollowings.Delete(d);
            }

            uow.Commit();
        }

        [TestMethod]
        public void Should_Be_Able_To_Remove_Following_User()
        {
            var userManagement = new FlowUsersOperations(uow);
            userManagement.AddFollowingUser("cgrant", "rredford");
            userManagement.RemoveFollowingUser("cgrant", "rredford");
            var res = userManagement.GetUser("cgrant");

            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.Following.Count());
            Assert.IsFalse(res.Following.Any(f => f.UserName == "rredford"));
        }

        [TestMethod]
        public void Should_Be_Able_To_Update_User()
        {
            var userManagement = new FlowUsersOperations(uow);
            var user = new UserInfo();
            var r = new Random((int)DateTime.Now.Ticks);
            var note = r.Next(1000000).ToString();
            user.Note = note;
            user.UserName = "cgrant";

            userManagement.UpdateUser(user);
            var dbUser = uow.Users.First(u => u.Name == "cgrant");

            Assert.IsNotNull(dbUser);
            Assert.AreEqual(note, dbUser.Note);
        }

    }
}
