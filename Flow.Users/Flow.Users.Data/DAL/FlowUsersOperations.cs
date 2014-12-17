using System.Collections.Generic;
using System.Linq;
using Flow.Users.Data.Core.Interfaces;
using Flow.Users.Contract.Interface;
using Flow.Library.Security;
using Flow.Users.Data.Infrastructure;
using Flow.Users.Contract.Message;
using System;
using Flow.Users.Data.Core;

namespace Flow.Users.Data.DAL
{
    /// <summary>
    /// Implements IFlowUsersOperations
    /// </summary>
    public class FlowUsersOperations : IFlowUsersOperations
    {
        private readonly IFlowUsersUnitOfWork _unitOfWork;

        public FlowUsersOperations(IFlowUsersUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public FlowUsersOperations()
        {
        }

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="roles">Roles</param>
        /// <returns></returns>
        public IEnumerable<string> GetUsersByRoles(IEnumerable<string> roles)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetUsersByRoles(uow, roles);
                }
            }
            return GetUsersByRoles(_unitOfWork, roles);
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <returns>True if valid user and password match</returns>
        public bool AuthenticateUser(string user, string password)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return AuthenticateUser(uow, user, password);
                }
            }
            return AuthenticateUser(_unitOfWork, user, password);
        }

        /// <summary>
        /// Check if a valid user for the domain
        /// </summary>
        /// <param name="domain">Domain</param>
        /// <param name="user">User</param>
        /// <returns>True if user belong to domain</returns>
        public bool IsValidUser(string domain, string user)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return IsValidUser(uow, domain, user);
                }
            }
            return IsValidUser(_unitOfWork, domain, user);
        }

        /// <summary>
        /// Get Users By Domains
        /// </summary>
        /// <param name="domains">Domains</param>
        /// <returns>List of users</returns>
        public IEnumerable<string> GetUsersByDomains(IEnumerable<string> domains)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetUsersByDomains(uow, domains);
                }
            }
            return GetUsersByDomains(_unitOfWork, domains);
        }

        /// <summary>
        /// Get Users
        /// </summary>
        /// <param name="nameToSearch">nameToSearch</param>
        /// <returns>List of users</returns>
        public IEnumerable<UserInfo> GetUserNames(string nameToSearch)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetUserNames(uow, nameToSearch);
                }
            }
            return GetUserNames(_unitOfWork, nameToSearch);
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="roleToSearch">nameToSearch</param>
        /// <returns>List of users</returns>
        public IEnumerable<RoleInfo> GetRoles(string roleToSearch)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetRoles(uow, roleToSearch);
                }
            }
            return GetRoles(_unitOfWork, roleToSearch);
        }

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of domains</returns>
        public IEnumerable<string> GetDomainsForUser(string user)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetDomainsForUser(uow, user);
                }
            }
            return GetDomainsForUser(_unitOfWork, user);
        }

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPrimaryRole"></param>
        /// <returns>List of roles</returns>
        public IEnumerable<string> GetRolesForUser(string user, bool isPrimaryRole)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetRolesForUser(uow, user, isPrimaryRole);
                }
            }
            return GetRolesForUser(_unitOfWork, user, isPrimaryRole);
        }

        /// <summary>
        /// Get Domains and Roles For User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of roles</returns>
        public DomainRoleInfo GetDomainRoleForUser(string user)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetDomainRoleForUser(uow, user);
                }
            }
            return GetDomainRoleForUser(_unitOfWork, user);
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>UserInfo</returns>
        public UserInfo GetUser(string user)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    return GetUser(uow, user);
                }
            }
            return GetUser(_unitOfWork, user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>UserInfo</returns>
        public void UpdateUser(UserInfo user)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    UpdateUser(uow, user);
                    return;
                }
            }
            UpdateUser(_unitOfWork, user);
        }

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="follower">Follower</param>
        /// <param name="following">Following</param>
        public void AddFollowingUser(string follower, string following)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    AddFollowingUser(uow, follower, following);
                    return;
                }
            }
            AddFollowingUser(_unitOfWork, follower, following);
        }

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="follower"></param>
        /// <param name="following"></param>
        public void RemoveFollowingUser(string follower, string following)
        {
            if (_unitOfWork == null)
            {
                using (var uow = new FlowUsersUnitOfWork())
                {
                    RemoveFollowingUser(uow, follower, following);
                    return;
                }
            }
            RemoveFollowingUser(_unitOfWork, follower, following);
        }

        #region Private Methods

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="roles">Roles</param>
        /// <returns>List of string</returns>
        private IEnumerable<string> GetUsersByRoles(IFlowUsersUnitOfWork uow, IEnumerable<string> roles)
        {
            var q = uow.RoleUsers.Find(ru => roles.Contains(ru.Role.Name), ru => ru.User);

            return q.Select(ur => ur.User.Name).Distinct().ToList();
        }

        /// <summary>
        /// Is Valid User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="domain">Domain</param>
        /// <param name="user">User</param>
        /// <returns>True or False</returns>
        private bool IsValidUser(IFlowUsersUnitOfWork uow, string domain, string user)
        {
            var u = uow.DomainUsers.FirstOrDefault(du => (string.IsNullOrEmpty(domain) || du.Domain.Name.Equals(domain, StringComparison.OrdinalIgnoreCase)) && du.User.Name.Equals(user, StringComparison.OrdinalIgnoreCase) && du.User.IsActive);

            return u != null;
        }

        /// <summary>
        /// Get Users By Domains
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="domains">Domains</param>
        /// <returns>List of string</returns>
        private IEnumerable<string> GetUsersByDomains(IFlowUsersUnitOfWork uow, IEnumerable<string> domains)
        {
            var q = new List<string>();
            foreach (var d in domains)
            {
                string d1 = d;
                q.AddRange(uow.DomainUsers.Find(du => du.Domain.Name.Equals(d1, StringComparison.OrdinalIgnoreCase), du => du.Domain, du => du.User)
                    .Select(du => du.User.Name));
            }

            return q.Distinct();
        }

        /// <summary>
        /// Get User Names
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="nameToSearch"></param>
        /// <returns>List of string</returns>
        private IEnumerable<UserInfo> GetUserNames(IFlowUsersUnitOfWork uow, string nameToSearch)
        {
            var q = uow.Users.AsQueryable()
                .Where(u => u.Name.Contains(nameToSearch))
                .Select(u => new UserInfo
                {
                    UserName = u.Name,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Phone = u.WorkPhone,
                    Position = u.Position,
                    Department = u.Department,
                    Email = u.Email,
                    Gender = u.Gender,
                    Title = u.Title,
                    PhotoPath = u.PhotoPath
                });

            return q.ToList();
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="roleToSearch"></param>
        /// <returns>List of String</returns>
        private IEnumerable<RoleInfo> GetRoles(IFlowUsersUnitOfWork uow, string roleToSearch)
        {
            var roles = uow.Roles.AsQueryable()
                .Where(u => u.Name.Contains(roleToSearch))
                .Select(r => new RoleInfo
                {
                    RoleName = r.Name,
                    Description = r.Description
                }).ToList();

            foreach (var r in roles)
            {
                RoleInfo role = r;
                var users = uow.RoleUsers.AsQueryable()
                    .Where(ru => ru.Role.Name == role.RoleName)
                    .Select(u => u.User.Name)
                    .ToList();

                r.Users = users;
            }

            return roles.ToList();
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="user">User</param>
        /// <param name="password">password</param>
        /// <returns>True or False</returns>
        private bool AuthenticateUser(IFlowUsersUnitOfWork uow, string user, string password)
        {
            var domainUsers = uow.DomainUsers.Find(du => du.User.Name.Equals(user, StringComparison.OrdinalIgnoreCase) && du.User.IsActive,
                du => du.Domain, du => du.User).ToList();

            if (!domainUsers.Any())
            {
                return false;
            }
            return password == Encryption.Decrypt(domainUsers[0].User.Password);
        }

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="user">User</param>
        /// <returns>List of string</returns>
        private IEnumerable<string> GetDomainsForUser(IFlowUsersUnitOfWork uow, string user)
        {
            var q = uow.DomainUsers.Find(du => du.User.Name.Equals(user, StringComparison.OrdinalIgnoreCase), du => du.Domain, du => du.User)
                .Select(du => du.Domain.Name);

            return q.Distinct().ToArray();
        }

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="user">User</param>
        /// <param name="isPrimaryRole"></param>
        /// <returns>List of string</returns>
        private IEnumerable<string> GetRolesForUser(IFlowUsersUnitOfWork uow, string user, bool isPrimaryRole)
        {
            var q = uow.RoleUsers.Find(du => du.User.Name.Equals(user, StringComparison.OrdinalIgnoreCase) && (!isPrimaryRole || du.IsPrimary), du => du.User, du => du.Role);

            if (q == null)
                return new List<string>().ToArray();

            return q.Select(r => r.Role.Name).ToArray();
        }

        /// <summary>
        /// Get Domain Role For User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="user">user</param>
        /// <returns>DomainRoleInfo</returns>
        private DomainRoleInfo GetDomainRoleForUser(IFlowUsersUnitOfWork uow, string user)
        {
            var domainRoleInfos = new DomainRoleInfo
            {
                Domanis = GetDomainsForUser(uow, user),
                Roles = GetRolesForUser(uow, user, false)
            };

            return domainRoleInfos;
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="user">User</param>
        /// <returns>UserInfo</returns>
        private UserInfo GetUser(IFlowUsersUnitOfWork uow, string user)
        {
            var dbuser = uow.Users.First(u => u.Name.Equals(user, StringComparison.OrdinalIgnoreCase) && u.IsActive);

            var follows = uow.UserFollowings.Find(uf => uf.FollowerUser.Name.Equals(user, StringComparison.OrdinalIgnoreCase), uf => uf.FollowingUser, uf => uf.FollowerUser)
                .Select(uf => new UserInfo
                {
                    UserName = uf.FollowingUser.Name,
                    Email = uf.FollowingUser.Email,
                    FirstName = uf.FollowingUser.FirstName,
                    LastName = uf.FollowingUser.LastName,
                    Gender = uf.FollowingUser.Gender,
                    Title = uf.FollowingUser.Title,
                    PhotoPath = uf.FollowingUser.PhotoPath,
                    Phone = uf.FollowingUser.WorkPhone,
                    Position = uf.FollowingUser.Position,
                    Department = uf.FollowingUser.Department
                })
                .ToList();

            var followers = uow.UserFollowings.Find(uf => uf.FollowingUser.Name.Equals(user, StringComparison.OrdinalIgnoreCase), uf => uf.FollowingUser, uf => uf.FollowerUser)
                .Select(uf => new UserInfo
                {
                    UserName = uf.FollowerUser.Name,
                    Email = uf.FollowerUser.Email,
                    FirstName = uf.FollowerUser.FirstName,
                    LastName = uf.FollowerUser.LastName,
                    Gender = uf.FollowerUser.Gender,
                    Title = uf.FollowerUser.Title,
                    PhotoPath = uf.FollowerUser.PhotoPath,
                    Phone = uf.FollowerUser.WorkPhone,
                    Position = uf.FollowerUser.Position,
                    Department = uf.FollowerUser.Department
                })
                .ToList();

            var userRoles = uow.RoleUsers.Find(ru => ru.User.Name.Equals(user, StringComparison.OrdinalIgnoreCase)).Select(ur => ur.RoleId);

            var pageStr = ResourceType.Page.ToString();
            var pages =
                uow.Resources.Find(r => r.Type == pageStr && (!r.RoleId.HasValue || userRoles.Contains(r.RoleId.Value)))
                .Distinct()
                .Select(r => new UserPages
                    {
                        Name = r.Display,
                        Url = r.Value,
                        Order = r.Order
                    })
                .ToList();

            //var pages = new List<UserPages>
            //{
            //    new UserPages {Url = "#", Name = "Home", Order = 1},
            //    new UserPages {Url = "#/tasks", Name = "Task", Order = 2},
            //    new UserPages {Url = "#/dashboard", Name = "Dashboard", Order = 3},
            //    new UserPages {Url = "#/sketch", Name = "Sketch", Order = 4},
            //    new UserPages {Url = "#/holidays", Name = "Holiday", Order = 5}
            //};

            return new UserInfo
            {
                UserName = dbuser.Name,
                Email = dbuser.Email,
                FirstName = dbuser.FirstName,
                LastName = dbuser.LastName,
                Gender = dbuser.Gender,
                Title = dbuser.Title,
                PhotoPath = dbuser.PhotoPath,
                Following = follows,
                Follower = followers,
                Birthday = dbuser.Birthday,                
                Note = dbuser.Note,
                Phone = dbuser.WorkPhone,
                Position = dbuser.Position,
                Department = dbuser.Department,
                FollowerCount = followers.Count,
                FollowingCount = follows.Count,
                UserPages = pages
            };
        }

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="uow">IFlowUsersUnitOfWork</param>
        /// <param name="follower">Follower</param>
        /// <param name="following">Following</param>
        private void AddFollowingUser(IFlowUsersUnitOfWork uow, string follower, string following)
        {
            var followings = uow.UserFollowings.Find(uf => uf.FollowerUser.Name.Equals(follower, StringComparison.OrdinalIgnoreCase), uf => uf.FollowingUser).ToList();

            if (!followings.Any(f => f.FollowingUser.Name.Equals(following, StringComparison.OrdinalIgnoreCase)))
            {
                var userFollower = uow.Users.First(u => u.Name.Equals(follower, StringComparison.OrdinalIgnoreCase));
                var userFollowing = uow.Users.First(u => u.Name.Equals(following, StringComparison.OrdinalIgnoreCase));

                uow.UserFollowings.Insert(new UserFollowing
                {
                    FollowerUser = userFollower,
                    FollowingUser = userFollowing
                });

                uow.Commit();
            }
        }

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="uow">IFlowUsersUnitOfWork</param>
        /// <param name="follower">Follower</param>
        /// <param name="following">Following</param>
        private void RemoveFollowingUser(IFlowUsersUnitOfWork uow, string follower, string following)
        {
            var followings = uow.UserFollowings.Find(uf => uf.FollowerUser.Name.Equals(follower, StringComparison.OrdinalIgnoreCase), uf => uf.FollowingUser).ToList();
            foreach (var f in followings)
            {
                if (f.FollowingUser.Name.Equals(following, StringComparison.OrdinalIgnoreCase))
                {
                    uow.UserFollowings.Delete(f);
                    uow.Commit();
                    break;
                }
            }
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="uow">IFlowUsersUnitOfWork</param>
        /// <param name="user">User</param>
        private void UpdateUser(IFlowUsersUnitOfWork uow, UserInfo user)
        {
            var dbUser = uow.Users.FirstOrDefault(u => u.Name.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) && u.IsActive);

            if (dbUser != null)
            {
                if (user.Birthday != null)
                {
                    dbUser.Birthday = user.Birthday;
                }
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    dbUser.Email = user.Email;
                }
                if (!string.IsNullOrWhiteSpace(user.Note))
                {
                    dbUser.Note = user.Note;
                }
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    dbUser.Password = Encryption.Encrypt(user.Password);
                }
                if (!string.IsNullOrWhiteSpace(user.Phone))
                {
                    dbUser.WorkPhone = user.Phone;
                }
                if (!string.IsNullOrWhiteSpace(user.PhotoPath))
                {
                    dbUser.PhotoPath = user.PhotoPath;
                }
                if (!string.IsNullOrWhiteSpace(user.Gender))
                {
                    dbUser.Gender = user.Gender;
                }
                if (!string.IsNullOrWhiteSpace(user.Department))
                {
                    dbUser.Department = user.Department;
                }
                if (!string.IsNullOrWhiteSpace(user.Position))
                {
                    dbUser.Position = user.Position;
                }

                uow.Commit();
            }
        }

        #endregion
    }
}
