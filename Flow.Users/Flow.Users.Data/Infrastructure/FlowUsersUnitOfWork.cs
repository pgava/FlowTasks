using Flow.Library;
using Flow.Library.EF;
using Flow.Users.Data.Core.Interfaces;
using Flow.Library.Interfaces;
using Flow.Users.Data.Core;

namespace Flow.Users.Data.Infrastructure
{
    /// <summary>
    /// FlowUsers Unit Of Work
    /// </summary>
    public class FlowUsersUnitOfWork : UnitOfWork, IFlowUsersUnitOfWork
    {
        Repository<User> _users;
        Repository<AddressDetails> _addresses;
        Repository<Domain> _domains;
        Repository<Role> _roles;
        Repository<DomainUser> _domainUsers;
        Repository<RoleUser> _roleUsers;
        Repository<OnlineStatus> _onlineStatuses;
        Repository<UserFollowing> _userFollowings;
        Repository<Resource> _resources;

        readonly IObjectSetFactory _contextAdapter;

        public FlowUsersUnitOfWork(IObjectSetFactory contextAdapter)
            : base(contextAdapter)
        {
            _contextAdapter = contextAdapter;
        }

        public FlowUsersUnitOfWork()
        {
            _contextAdapter = new FlowUsersContextAdapter(new FlowUsersEntities());
            ObjectContext = _contextAdapter;
        }

        #region IFlow.Users.Data.nitOfWork Members

        public IRepository<User> Users
        {
            get { return _users ?? (_users = new Repository<User>(_contextAdapter)); }
        }

        public IRepository<Domain> Domains
        {
            get { return _domains ?? (_domains = new Repository<Domain>(_contextAdapter)); }
        }

        public IRepository<Role> Roles
        {
            get { return _roles ?? (_roles = new Repository<Role>(_contextAdapter)); }
        }

        public IRepository<DomainUser> DomainUsers
        {
            get { return _domainUsers ?? (_domainUsers = new Repository<DomainUser>(_contextAdapter)); }
        }

        public IRepository<RoleUser> RoleUsers
        {
            get { return _roleUsers ?? (_roleUsers = new Repository<RoleUser>(_contextAdapter)); }
        }

        public IRepository<AddressDetails> Addresses
        {
            get { return _addresses ?? (_addresses = new Repository<AddressDetails>(_contextAdapter)); }
        }

        public IRepository<OnlineStatus> OnlineStatuses
        {
            get { return _onlineStatuses ?? (_onlineStatuses = new Repository<OnlineStatus>(_contextAdapter)); }
        }

        public IRepository<UserFollowing> UserFollowings
        {
            get { return _userFollowings ?? (_userFollowings = new Repository<UserFollowing>(_contextAdapter)); }
        }

        public IRepository<Resource> Resources
        {
            get { return _resources ?? (_resources = new Repository<Resource>(_contextAdapter)); }
        }

        #endregion
    }
}
