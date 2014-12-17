
using Flow.Library;

namespace Flow.Users.Data.Migrations
{
    using Core;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<FlowUsersEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FlowUsersEntities context)
        {

            var address1 = CreateAddress();

            //var status1 = CreateStatus("Online", "User is online");
            //var status2 = CreateStatus("Offline", "User is offline");
            //var status3 = CreateStatus("Busy", "User is busy");

            context.Addresses.AddOrUpdate(
                addr => addr.AddressDetailsId,
                address1
                );

            context.OnlineStatuses.AddOrUpdate(
                os => os.Code,
                CreateStatus("Online", "User is online"),
                CreateStatus("Offline", "User is offline"),
                CreateStatus("Busy", "User is busy")
                );

            var user1 = CreateUser("cgrant", "Cary", "Grant", "cgrant@acmestar.com", "Images\\users\\cgrant.jpg", "Developer Manager", "IT");
            var user2 = CreateUser("pnewman", "Paul", "Newman", "pnewman@acmestar.com", "Images\\users\\pnewman.jpg", "Developer", "IT");
            var user3 = CreateUser("rredford", "Robert", "Redford", "rredford@acmestar.com", "Images\\users\\rredford.jpg", "Developer", "IT");
            var user4 = CreateUser("hbogart", "Humphrey", "Bogart", "hbogart@acmestar.com", "Images\\users\\hbogart.jpg", "Business Analyst Manager", "Finance");
            var user5 = CreateUser("knovak", "Kim", "Novak", "knovak@acmestar.com", "Images\\users\\knovak.jpg", "Business Analyst", "Finance");
            var user6 = CreateUser("mmonroe", "Marilyn", "Monroe", "mmonroe@acmestar.com", "Images\\users\\mmonroe.jpg", "Accountant", "Finance");
            var user7 = CreateUser("apacino", "Al", "Pacino", "apacino@acmestar.com", "Images\\users\\apacino.jpg", "Finance Manager", "Finance");
            var user8 = CreateUser("cbronson", "Charles", "Bronson", "cbronson@acmestar.com", "Images\\users\\cbronson.jpg", "Developer", "IT");
            var user9 = CreateUser("dday", "Doris", "Day", "dday@acmestar.com", "Images\\users\\dday.jpg", "Accountant", "Finance");
            var user10 = CreateUser("gkelly", "Grace", "Kelly", "gkelly@acmestar.com", "Images\\users\\gkelly.jpg", "Business Analyst", "Finance");
            var user11 = CreateUser("gpeck", "Gregory", "Peck", "gpeck@acmestar.com", "Images\\users\\gpeck.jpg", "Project Manager", "Finance");
            var user12 = CreateUser("jdean", "James", "Dean", "jdean@acmestar.com", "Images\\users\\jdean.jpg", "Project Manager", "IT");
            var user13 = CreateUser("jstewart", "James", "Stewart", "jstewart@acmestar.com", "Images\\users\\jstewart.jpg", "PM Manager", "IT");
            var user14 = CreateUser("mpfeiffer", "Michelle", "Pfeiffer", "mpfeiffer@acmestar.com", "Images\\users\\mpfeiffer.jpg", "Business Analyst", "IT");
            var user15 = CreateUser("nkidman", "Nicole", "Kidman", "nkidman@acmestar.com", "Images\\users\\nkidman.jpg", "Human Resources", "HR");
            var user16 = CreateUser("rdeniro", "Robert", "Deniro", "rdeniro@acmestar.com", "Images\\users\\rdeniro.jpg", "Developer", "IT");
            var user17 = CreateUser("rhayworth", "Rita", "Hayworth", "rhayworth@acmestar.com", "Images\\users\\rhayworth.jpg", "Project Manager", "Finance");
            var user18 = CreateUser("rmitchum", "Robert", "Mitchum", "rmitchum@acmestar.com", "Images\\users\\rmitchum.jpg", "Developer", "IT");
            var user19 = CreateUser("sdee", "Sandra", "Dee", "sdee@acmestar.com", "Images\\users\\sdee.jpg", "Accountant", "Finance");
            var user20 = CreateUser("smcqueen", "Steve", "Mcqueen", "smcqueen@acmestar.com", "Images\\users\\smcqueen.jpg", "Developer", "IT");
            var user21 = CreateUser("tcruise", "Tom", "Cruise", "tcruise@acmestar.com", "Images\\users\\tcruise.jpg", "Human Resources Manager", "HR");

            context.Users.AddOrUpdate(
                u => u.Name,
                user1,
                user2,
                user3,
                user4,
                user5,
                user6,
                user7,
                user8,
                user9,
                user10,
                user11,
                user12,
                user13,
                user14,
                user15,
                user16,
                user17,
                user18,
                user19,
                user20,
                user21
                );

            var role1 = CreateRole("Dev");
            var role2 = CreateRole("Approver");
            var role3 = CreateRole("Finance");
            var role4 = CreateRole("MgrFinance");
            var role5 = CreateRole("Admin");
            var role6 = CreateRole("PM");
            var role7 = CreateRole("BA");
            var role8 = CreateRole("HR");
            var role9 = CreateRole("MgrDev");
            var role10 = CreateRole("MgrPM");
            var role11 = CreateRole("MgrBA");
            var role12 = CreateRole("MgrHR");
            
            context.Roles.AddOrUpdate(
                r => r.Name,
                role1,
                role2,
                role3,
                role4,
                role5,
                role6,
                role7,
                role8,
                role9,
                role10,
                role11,
                role12
            );

            var domain1 = CreateDomain("AcmeStar");

            context.Domains.AddOrUpdate(
                d => d.Name,
                domain1
                );

            var rsc1 = CreateResource("Home page for all users", "page", "Home", "#", 1, null);
            var rsc2 = CreateResource("Tasks page for all users", "page", "Task", "#/tasks", 2, null);
            var rsc3 = CreateResource("Dashboard page for Admin", "page", "Dashboard", "#/dashboard", 3, role5);
            var rsc4 = CreateResource("Sketch page for BA", "page", "Sketch", "#/sketch", 4, role7);
            var rsc5 = CreateResource("Sketch page for BA manager", "page", "Sketch", "#/sketch", 4, role11);
            var rsc6 = CreateResource("Holiday page for all users", "page", "Holidays", "#/holidays", 6, null);
            var rsc7 = CreateResource("Report page for PM", "page", "Report", "#/report", 5, role6);
            var rsc8 = CreateResource("Report page for PM manager", "page", "Report", "#/report", 5, role10);

            context.Resources.AddOrUpdate(
                r => r.Description,
                rsc1,
                rsc2,
                rsc3,
                rsc4,
                rsc5,
                rsc6,
                rsc7,
                rsc8
                );

            // cgrant-AcmeStar
            var domainUser1 = CreateDomainUser(domain1, user1);

            // pnewman-AcmeStar
            var domainUser2 = CreateDomainUser(domain1, user2);

            // rredford-AcmeStar
            var domainUser3 = CreateDomainUser(domain1, user3);

            // hbogart-AcmeStar
            var domainUser4 = CreateDomainUser(domain1, user4);

            // knovak-AcmeStar
            var domainUser5 = CreateDomainUser(domain1, user5);

            // mmonroe-AcmeStar
            var domainUser6 = CreateDomainUser(domain1, user6);

            // apacino-AcmeStar
            var domainUser7 = CreateDomainUser(domain1, user7);

            // cbronson-AcmeStar
            var domainUser8 = CreateDomainUser(domain1, user8);

            // dday-AcmeStar
            var domainUser9 = CreateDomainUser(domain1, user9);

            // gkelly-AcmeStar
            var domainUser10 = CreateDomainUser(domain1, user10);

            // gpeck-AcmeStar
            var domainUser11 = CreateDomainUser(domain1, user11);

            // jdean-AcmeStar
            var domainUser12 = CreateDomainUser(domain1, user12);

            // jstewart-AcmeStar
            var domainUser13 = CreateDomainUser(domain1, user13);

            // mpfeiffer-AcmeStar
            var domainUser14 = CreateDomainUser(domain1, user14);

            // nkidman-AcmeStar
            var domainUser15 = CreateDomainUser(domain1, user15);

            // rdeniro-AcmeStar
            var domainUser16 = CreateDomainUser(domain1, user16);

            // rhayworth-AcmeStar
            var domainUser17 = CreateDomainUser(domain1, user17);

            // rmitchum-AcmeStar
            var domainUser18 = CreateDomainUser(domain1, user18);

            // sdee-AcmeStar
            var domainUser19 = CreateDomainUser(domain1, user19);

            // smcqueen-AcmeStar
            var domainUser20 = CreateDomainUser(domain1, user20);

            // tcruise-AcmeStar
            var domainUser21 = CreateDomainUser(domain1, user21);

            // cgrant-MgrDev
            var roleUser1 = CreateRoleUser(user1, role9, true);

            // cgrant-Admin
            var roleUser2 = CreateRoleUser(user1, role5, false);

            // cgrant-Approver
            var roleUser4 = CreateRoleUser(user1, role2, false);

            // pnewman-Dev
            var roleUser5 = CreateRoleUser(user2, role1, true);

            // pnewman-Admin
            var roleUser6 = CreateRoleUser(user2, role5, false);

            // rredford-Dev
            var roleUser7 = CreateRoleUser(user3, role1, true);

            // rredford-Admin
            var roleUser8 = CreateRoleUser(user3, role5, false);

            // hbogart-MgrBA
            var roleUser9 = CreateRoleUser(user4, role11, true);

            // knovak-BA
            var roleUser10 = CreateRoleUser(user5, role7, true);

            // mmonroe-Finance
            var roleUser11 = CreateRoleUser(user6, role3, true);

            // apacino-MgrFinance
            var roleUser12 = CreateRoleUser(user7, role4, true);

            // cbronson-Finance
            var roleUser13 = CreateRoleUser(user8, role1, true);

            // dday-Finance
            var roleUser14 = CreateRoleUser(user9, role3, true);

            // gkelly-BA
            var roleUser15 = CreateRoleUser(user10, role7, true);

            // gpeck-PM
            var roleUser16 = CreateRoleUser(user11, role6, true);

            // jdean-PM
            var roleUser17 = CreateRoleUser(user12, role6, true);

            // jstewart-MgrPM
            var roleUser18 = CreateRoleUser(user13, role10, true);

            // mpfeiffer-BA
            var roleUser19 = CreateRoleUser(user14, role7, true);

            // nkidman-HR
            var roleUser20 = CreateRoleUser(user15, role8, true);

            // rdeniro-Dev
            var roleUser21 = CreateRoleUser(user16, role1, true);

            // rdeniro-Approver
            var roleUser22 = CreateRoleUser(user16, role2, false);

            // rhayworth-PM
            var roleUser23 = CreateRoleUser(user17, role6, true);

            // rmitchum-Dev
            var roleUser24 = CreateRoleUser(user18, role1, true);

            // sdee-Finance
            var roleUser25 = CreateRoleUser(user19, role3, true);

            // smcqueen-Dev
            var roleUser26 = CreateRoleUser(user20, role1, true);

            // tcruise-MgrHR
            var roleUser27 = CreateRoleUser(user21, role12, true);

            context.UserFollowings.AddOrUpdate(
                uf => new {uf.FollowerUserId,uf.FollowingUserId},
                CreateFollownig(user1, user2),
                CreateFollownig(user2, user1),
                CreateFollownig(user1, user4),
                CreateFollownig(user1, user5),
                CreateFollownig(user5, user6)
                );            
            
            context.DomainUsers.AddOrUpdate(
                du => new { du.DomainId, du.UserId},
                domainUser1,
                domainUser2,
                domainUser3,
                domainUser4,
                domainUser5,
                domainUser6,
                domainUser7,
                domainUser8,
                domainUser9,
                domainUser10,
                domainUser11,
                domainUser12,
                domainUser13,
                domainUser14,
                domainUser15,
                domainUser16,
                domainUser17,
                domainUser18,
                domainUser19,
                domainUser20,
                domainUser21
                );

            context.RoleUsers.AddOrUpdate(
                ru => new { ru.RoleId, ru.UserId },
                roleUser1,
                roleUser2,
                //roleUser3,
                roleUser4,
                roleUser5,
                roleUser6,
                roleUser7,
                roleUser8,
                roleUser9,
                roleUser10,
                roleUser11,
                roleUser12,
                roleUser13,
                roleUser14,
                roleUser15,
                roleUser16,
                roleUser17,
                roleUser18,
                roleUser19,
                roleUser20,
                roleUser21,
                roleUser22,
                roleUser23,
                roleUser24,
                roleUser25,
                roleUser26,
                roleUser27
                );            
        }

        private User CreateUser(string name, string firstName, string lastName, string email, string photoPath,
            string position, string department)
        {
            return new User
            {
                Name = name,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = "I/Wn9e+RVG0=",
                IsActive = true,
                PhotoPath = photoPath,
                Position = position,
                Department = department,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
                
            };
        }
        private Role CreateRole(string name)
        {
            return new Role
            {
                Name = name,
                Description = "Role description",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
        }
        private Domain CreateDomain(string name)
        {
            return new Domain
            {
                Name = name,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
        }

        private DomainUser CreateDomainUser(Domain d, User u)
        {
            if (d.DomainId != 0)
            {
                return new DomainUser { DomainId = d.DomainId, UserId = u.UserId };
            }
            return new DomainUser { Domain = d, User = u };
        }

        private RoleUser CreateRoleUser(User u, Role r, bool isPrimary)
        {
            if (u.UserId != 0)
            {
                return new RoleUser { RoleId = r.RoleId, UserId = u.UserId, IsPrimary = isPrimary };
            }

            return new RoleUser { Role = r, User = u, IsPrimary = isPrimary };
        }

        private OnlineStatus CreateStatus(string code, string desc)
        {
            var os = new OnlineStatus { Code = code, Description = desc };
            return os;
        }

        private AddressDetails CreateAddress()
        {
            var ad = new AddressDetails { Address = "11 aa", City = "Macondo", Country = "America", PostalCode = "21456", Region = "America" };
            return ad;
        }

        private UserFollowing CreateFollownig(User follower, User following)
        {
            if (follower.UserId != 0)
            {
                return new UserFollowing { FollowerUserId = follower.UserId, FollowingUserId = following.UserId };                
            }
            return new UserFollowing { FollowerUser = follower, FollowingUser = following };
        }

        private Resource CreateResource(string description, string type, string display, string value, int order, Role role)
        {
            if (role != null && role.RoleId != 0)
            {
                return new Resource { Description = description, Type = type, Display = display, Value = value, Order = order, RoleId = role.RoleId };                
            }
            return new Resource { Description = description, Type = type, Display = display, Value = value, Order = order, Role = role };
        }
    }
}
