using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;
using Flow.Tasks.Data.Core;

namespace Flow.Tasks.Data
{
    /// <summary>
    /// FlowTasks Entities
    /// </summary>
    public sealed class FlowTasksEntities : DbContext
    {
        public DbSet<WorkflowCode> WorkflowCodes { get; set; }
        public DbSet<WorkflowConfiguration> WorkflowConfigurations { get; set; }
        public DbSet<TaskConfiguration> TaskConfigurations { get; set; }
        public DbSet<WorkflowProperty> WorkflowProperties { get; set; }
        public DbSet<TaskProperty> TaskProperties { get; set; }
        public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
        public DbSet<TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<TaskInParameter> TaskInParameters { get; set; }
        public DbSet<WorkflowInParameter> WorkflowInParameters { get; set; }
        public DbSet<TaskOutParameter> TaskOutParameters { get; set; }
        public DbSet<WorkflowOutParameter> WorkflowOutParameters { get; set; }
        public DbSet<TraceEvent> TraceEvents { get; set; }
        public DbSet<WorkflowTrace> WorkflowTraces { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }
        public DbSet<TaskUserHandOver> TaskUserHandOvers { get; set; }
        public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
        public DbSet<SketchStatus> SketchStatuses { get; set; }
        public DbSet<SketchConfiguration> SketchConfigurations { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicMessage> TopicMessages { get; set; }
        public DbSet<TopicUser> TopicUsers { get; set; }
        public DbSet<TopicAttachment> TopicAttachments { get; set; }
        public DbSet<TopicStatus> TopicStatuses { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayType> HolidayTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new WorkflowDefinitionConfiguration());
        }

        class WorkflowDefinitionConfiguration : EntityTypeConfiguration<WorkflowDefinition>
        {
            internal WorkflowDefinitionConfiguration()
            {
                HasOptional(w => w.WorkflowParentDefinition)
                    .WithMany()
                    .HasForeignKey(w => w.WorkflowParentDefinitionId)
                    .WillCascadeOnDelete(false);
            }
        }

        /// <summary>
        /// TERRIBLE HACK TO MAKE AUTOMATED MSTEST TO WORK AGAIN WITH EF6
        /// </summary>
        static FlowTasksEntities()
        {
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }

    }
}
