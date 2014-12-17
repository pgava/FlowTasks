using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Flow.Library.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;

using Flow.Library.Interfaces;
using Flow.Library;
using Flow.Users.Data;
using Flow.Users.Data.Core;
using Flow.Users.Data.Infrastructure;
using Flow.Library.EF;

namespace UsersFlowTest
{
    [TestClass]
    public sealed class UsersRepositoryTest : IDisposable
    {
        private FlowUsersEntities ctx;
        private DbContextAdapter adp;
        private FlowUsersUnitOfWork uow;

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

        [TestInitialize]
        public void CreateContext()
        {
            ctx = new FlowUsersEntities();
            adp = new FlowUsersContextAdapter(ctx);
            uow = new FlowUsersUnitOfWork(adp);
        }

        private TestContext testContextInstance;

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


        [TestMethod]
        public void ShouldAuthenticateUser()
        {
            string pwd = Encryption.Encrypt("pwd");
            string usr = "cgrant";

            /*
            UsersFlow.UsersFlowEntities sut = new UsersFlow.UsersFlowEntities();

            var lst = from u in sut.Users
                        where u.Name == usr && u.Password == pwd
                        select u;
            */
            
            var q = uow.Users.AsQueryable()
                .Where(u => u.Name == usr && u.Password == pwd);

            var lst = q.ToList();

            Assert.IsTrue(lst.Count() > 0);
            Assert.IsTrue(lst.First().Name == "cgrant");
        }

        [TestMethod]
        public void ShouldNotAuthenticateUserWhenPasswordWrong()
        {
            string pwd = Encryption.Encrypt("pwd1");
            string usr = "cgrant";

            /*
            UsersFlow.UsersFlowEntities sut = new UsersFlow.UsersFlowEntities();

            var lst = from u in sut.Users
                        where u.Name == usr && u.Password == pwd
                        select u;
            */

            var q = uow.Users.AsQueryable()
                .Where(u => u.Name == usr && u.Password == pwd);

            var lst = q.ToList();

            Assert.IsTrue(lst.Count() == 0);
        }

        [TestMethod]
        public void ShouldGetUsersByDomain()
        {
            string dmn = "AcmeStar";

            /*
            Expression<Func<DomainUser, object>>[] includeProperties = 
                new Expression<Func<DomainUser, object>>[] { d => d.Domain, u => u.User };

            IEnumerable<DomainUser> lst = _domainUserRepo.Find(
                du => du.Domain.Name == dmn, includeProperties);
            */

            var q = uow.DomainUsers.AsQueryable()
                .Include("Domain")
                .Include("User")
                .Where(du => du.Domain.Name == dmn).Select(du => du.User.Name);
            
            var lst = q.Distinct().ToList();

            Assert.AreEqual(21, lst.Count());
            Assert.IsTrue(lst.Contains("cgrant"));
            Assert.IsTrue(lst.Contains("pnewman"));
            Assert.IsTrue(lst.Contains("hbogart"));
            Assert.IsTrue(lst.Contains("knovak"));
            Assert.IsTrue(lst.Contains("mmonroe"));
        }

        [TestMethod]
        public void ShouldGetUsersByRole()
        {
            var q = from r in uow.RoleUsers.AsQueryable()
                    where r.Role.Name == "Dev"
                    select r.User;

            var lst = q.ToList();

            Assert.AreEqual(6, lst.Count());

            var names = string.Join(",", lst.Select(u => u.Name).ToArray());

            Assert.IsTrue(names.Contains("pnewman"));
                
        }

    }
}
