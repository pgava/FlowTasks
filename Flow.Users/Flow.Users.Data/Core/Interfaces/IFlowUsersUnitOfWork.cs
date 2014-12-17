using Flow.Library.Interfaces;

namespace Flow.Users.Data.Core.Interfaces
{
    public interface IFlowUsersUnitOfWork : IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<AddressDetails> Addresses { get; }
        IRepository<Domain> Domains { get; }
        IRepository<Role> Roles { get; }
        IRepository<DomainUser> DomainUsers { get; }
        IRepository<RoleUser> RoleUsers { get; }
        IRepository<OnlineStatus> OnlineStatuses { get; }
        IRepository<UserFollowing> UserFollowings { get; }
        IRepository<Resource> Resources { get; }
    }
}
