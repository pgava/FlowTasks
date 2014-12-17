namespace Flow.Tasks.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullTextIndex : DbMigration
    {
        private const string FULL_TEXT_INDEX = @"
CREATE FULLTEXT CATALOG [Topic] WITH ACCENT_SENSITIVITY = ON AS DEFAULT

CREATE FULLTEXT INDEX ON TopicMessage(Message)
KEY INDEX [pk_dbo.TopicMessage] 
	ON Topic
";


        public override void Up()
        {
            Sql(FULL_TEXT_INDEX, true);
        }
        
        public override void Down()
        {
        }
    }
}
