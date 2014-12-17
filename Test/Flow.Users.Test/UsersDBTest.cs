using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Users.Data.Core;
using Flow.Users.Data;

namespace UsersFlowTest
{
    /// <summary>
    /// Summary description for UsersFlowDBTest
    /// </summary>
    [TestClass]
    public class UsersDBTest
    {

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

        private User CreateUser(string name, string email, string photoPath)
        {
            return new User()
            {
                Name = name,
                Email = email,
                Password = "pwd",
                IsActive = true,
                PhotoPath = photoPath,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
        }
        private Role CreateRole(string name, string desc)
        {
            return new Role()
            {
                Name = name,
                Description = "Role description",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
        }
        private Domain CreateDomain(string name)
        {
            return new Domain()
            {
                Name = name,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
        }

        private DomainUser CreateDomainUser(Domain d, User u)
        {
            var du = new DomainUser { Domain = d, User = u };
            return du;
        }

        private RoleUser CreateRoleUser(User u, Role r)
        {
            var ru = new RoleUser { Role = r, User = u };
            return ru;
        }

        private OnlineStatus CreateStatus(string code, string desc)
        {
            var os = new OnlineStatus { Code = code, Description = desc };
            return os;            
        }

        private AddressDetails CreateAddress()
        {
            var ad = new AddressDetails { Address = "11 aa", City = "Macondo", Country = "World", PostalCode = "", Region = "Earth" };
            return ad;
        }

        [TestMethod]
        [Ignore]
        public void CreateDb()
        {
            var address1 = CreateAddress();

            var status1 = CreateStatus("Online", "User is online");
            var status2 = CreateStatus("Offline", "User is offline");
            var status3 = CreateStatus("Busy", "User is busy");
            
            var user1 = CreateUser("cgrant", "cgrant@aa.com", "Images\\users\\user1.png");
            var user2 = CreateUser("pnewman", "pnewman@aa.com", "Images\\users\\user1.png");
            var user3 = CreateUser("rredford", "rredford@aa.com", "Images\\users\\user1.png");
            var user4 = CreateUser("hbogart", "hbogart@aa.com", "Images\\users\\user1.png");
            var user5 = CreateUser("knovak", "knovak@aa.com", "Images\\users\\user.png");
            var user6 = CreateUser("mmonroe", "mmonroe@aa.com", "Images\\users\\user.png");

            var role1 = CreateRole("Dev", "Developer");
            var role2 = CreateRole("Approver", "Approve task");
            var role3 = CreateRole("Finance", "Finance department");
            var role4 = CreateRole("Guest", "Guest");
            var role5 = CreateRole("Admin", "Administrator");
            var role6 = CreateRole("PM", "Project manager");
            var role7 = CreateRole("BA", "Business analyst");
            var domain1 = CreateDomain("google");
            var domain2 = CreateDomain("microsoft");
            var domain3 = CreateDomain("CommonWorkGroup");

            // cgrant-google
            var domainUser1 = CreateDomainUser(domain1, user1);

            // pnewman-google
            var domainUser2 = CreateDomainUser(domain1, user2);

            // rredford-microsoft
            var domainUser3 = CreateDomainUser(domain2, user3);

            // hbogart-google
            var domainUser4 = CreateDomainUser(domain1, user4);

            // knovak-google
            var domainUser5 = CreateDomainUser(domain1, user5);

            // mmonroe-google
            var domainUser6 = CreateDomainUser(domain1, user6);

            // cgrant-Dev
            var roleUser1 = CreateRoleUser(user1, role1);

            // cgrant-Admin
            var roleUser2 = CreateRoleUser(user1, role5);

            // cgrant-PM
            var roleUser3 = CreateRoleUser(user1, role6);

            // cgrant-Approver
            var roleUser4 = CreateRoleUser(user1, role2);

            // pnewman-Dev
            var roleUser5 = CreateRoleUser(user2, role1);

            // rredford-Approver
            var roleUser6 = CreateRoleUser(user3, role2);

            // hbogart-Finance
            var roleUser7 = CreateRoleUser(user4, role3);

            // hbogart-BA
            var roleUser8 = CreateRoleUser(user4, role7);

            // knovak-Finance
            var roleUser9 = CreateRoleUser(user5, role3);

            // mmonroe-Finance
            var roleUser10 = CreateRoleUser(user6, role3);

            using (var ctx = new FlowUsersEntities())
            {
                ctx.Addresses.Add(address1);
                ctx.OnlineStatuses.Add(status1);
                ctx.OnlineStatuses.Add(status2);
                ctx.OnlineStatuses.Add(status3);
                ctx.Users.Add(user1);
                ctx.Users.Add(user2);
                ctx.Users.Add(user3);
                ctx.Users.Add(user4);
                ctx.Users.Add(user5);
                ctx.Users.Add(user6);
                ctx.Domains.Add(domain1);
                ctx.Domains.Add(domain2);
                ctx.Roles.Add(role1);
                ctx.Roles.Add(role2);
                ctx.Roles.Add(role3);
                ctx.Roles.Add(role4);
                ctx.DomainUsers.Add(domainUser1);
                ctx.DomainUsers.Add(domainUser2);
                ctx.DomainUsers.Add(domainUser3);
                ctx.DomainUsers.Add(domainUser4);
                ctx.DomainUsers.Add(domainUser5);
                ctx.DomainUsers.Add(domainUser6);
                ctx.RoleUsers.Add(roleUser1);
                ctx.RoleUsers.Add(roleUser2);
                ctx.RoleUsers.Add(roleUser3);
                ctx.RoleUsers.Add(roleUser4);
                ctx.RoleUsers.Add(roleUser5);
                ctx.RoleUsers.Add(roleUser6);
                ctx.RoleUsers.Add(roleUser7);
                ctx.RoleUsers.Add(roleUser8);
                ctx.RoleUsers.Add(roleUser9);
                ctx.RoleUsers.Add(roleUser10);

                ctx.SaveChanges();
            }
        }

    }
}
