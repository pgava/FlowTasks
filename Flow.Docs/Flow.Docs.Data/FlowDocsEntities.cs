using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Flow.Docs.Data.Core;
using System.Data.Entity.ModelConfiguration;

namespace Flow.Docs.Data
{
    /// <summary>
    /// FlowDocs Entities
    /// </summary>
    public class FlowDocsEntities : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new AttachmentConfiguration());
            modelBuilder.Configurations.Add(new DocumentConfiguration());                
        }

        /// <summary>
        /// TERRIBLE HACK TO MAKE AUTOMATED MSTEST TO WORK AGAIN WITH EF6
        /// </summary>
        static FlowDocsEntities()
        {
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }

    }

    class AttachmentConfiguration : EntityTypeConfiguration<Attachment>
    {
        internal AttachmentConfiguration()
        {
            HasOptional(a => a.Document)
                .WithMany()
                .HasForeignKey(a => a.DocumentId)
                .WillCascadeOnDelete(true);
        }
    }

    class DocumentConfiguration : EntityTypeConfiguration<Document>
    {
        internal DocumentConfiguration()
        {
            HasOptional(d => d.DocumentPrevious)
                .WithMany()
                .HasForeignKey(d => d.DocumentPreviousId)
                .WillCascadeOnDelete(false);
        }
    }
}
