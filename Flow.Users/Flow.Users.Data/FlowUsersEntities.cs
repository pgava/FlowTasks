using System.Data.Entity;
using Flow.Users.Data.Core;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;

namespace Flow.Users.Data
{
    /// <summary>
    /// FlowUsers Entities
    /// </summary>
    public class FlowUsersEntities : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AddressDetails> Addresses { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<DomainUser> DomainUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<OnlineStatus> OnlineStatuses { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new UserFollowingsConfiguration());

            modelBuilder.Configurations.Add(new DomainUserConfiguration());
        }

        /// <summary>
        /// TERRIBLE HACK TO MAKE AUTOMATED MSTEST TO WORK AGAIN WITH EF6
        /// </summary>
        static FlowUsersEntities()
        {
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }
    }

    class UserFollowingsConfiguration : EntityTypeConfiguration<UserFollowing>
    {
        internal UserFollowingsConfiguration()
        {
            HasRequired(u => u.FollowerUser)
                .WithMany()
                .HasForeignKey(u => u.FollowerUserId)
                .WillCascadeOnDelete(false);
            HasRequired(u => u.FollowingUser)
                .WithMany()
                .HasForeignKey(u => u.FollowingUserId)
                .WillCascadeOnDelete(false);
        }
    }

    class DomainUserConfiguration : EntityTypeConfiguration<DomainUser>
    {
        internal DomainUserConfiguration()
        {
            HasRequired(u => u.Domain)
                .WithMany()
                .HasForeignKey(u => u.DomainId)
                .WillCascadeOnDelete(false);
            HasRequired(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
