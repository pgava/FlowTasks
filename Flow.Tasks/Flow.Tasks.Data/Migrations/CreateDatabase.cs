namespace Flow.Tasks.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Holiday",
                c => new
                    {
                        HolidayId = c.Int(nullable: false, identity: true),
                        User = c.String(maxLength: 16),
                        Year = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 1),
                        HolidayTypeId = c.Int(nullable: false),
                        Dates = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.HolidayId)
                .ForeignKey("dbo.HolidayType", t => t.HolidayTypeId, cascadeDelete: true)
                .Index(t => t.HolidayTypeId);
            
            CreateTable(
                "dbo.HolidayType",
                c => new
                    {
                        HolidayTypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.HolidayTypeId);
            
            CreateTable(
                "dbo.Property",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Value = c.String(maxLength: 200),
                        Type = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.PropertyId);
            
            CreateTable(
                "dbo.SketchConfiguration",
                c => new
                    {
                        SketchConfigurationId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        XamlxOid = c.Guid(nullable: false),
                        LastSavedOn = c.DateTime(nullable: false),
                        ChangedBy = c.String(maxLength: 16),
                        SketchStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SketchConfigurationId)
                .ForeignKey("dbo.SketchStatus", t => t.SketchStatusId, cascadeDelete: true)
                .Index(t => t.SketchStatusId);
            
            CreateTable(
                "dbo.SketchStatus",
                c => new
                    {
                        SketchStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.SketchStatusId);
            
            CreateTable(
                "dbo.TaskConfiguration",
                c => new
                    {
                        TaskConfigurationId = c.Int(nullable: false, identity: true),
                        TaskCode = c.String(maxLength: 20),
                        Title = c.String(maxLength: 200),
                        Description = c.String(maxLength: 500),
                        CanBeHandedOver = c.Boolean(nullable: false),
                        HandOverUsers = c.String(maxLength: 200),
                        AssignedToUsers = c.String(maxLength: 200),
                        UiCode = c.String(maxLength: 20),
                        DefaultResult = c.String(maxLength: 20),
                        WorkflowCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskConfigurationId)
                .ForeignKey("dbo.WorkflowCode", t => t.WorkflowCodeId, cascadeDelete: true)
                .Index(t => t.WorkflowCodeId);
            
            CreateTable(
                "dbo.WorkflowCode",
                c => new
                    {
                        WorkflowCodeId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.WorkflowCodeId);
            
            CreateTable(
                "dbo.TaskDefinition",
                c => new
                    {
                        TaskDefinitionId = c.Int(nullable: false, identity: true),
                        TaskOid = c.Guid(nullable: false),
                        TaskCorrelationId = c.Int(nullable: false),
                        TaskCode = c.String(nullable: false, maxLength: 50),
                        UiCode = c.String(nullable: false, maxLength: 200),
                        Title = c.String(maxLength: 200),
                        Description = c.String(maxLength: 500),
                        DefaultResult = c.String(maxLength: 20),
                        ExpiryDate = c.DateTime(),
                        AcceptedOn = c.DateTime(),
                        AcceptedBy = c.String(maxLength: 16),
                        CompletedOn = c.DateTime(),
                        AcceptUser = c.String(maxLength: 16),
                        CanBeHandedOver = c.Boolean(nullable: false),
                        HandedOverStatus = c.String(maxLength: 20),
                        WorkflowDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskDefinitionId)
                .ForeignKey("dbo.WorkflowDefinition", t => t.WorkflowDefinitionId, cascadeDelete: true)
                .Index(t => t.WorkflowDefinitionId);
            
            CreateTable(
                "dbo.WorkflowDefinition",
                c => new
                    {
                        WorkflowDefinitionId = c.Int(nullable: false, identity: true),
                        WorkflowOid = c.Guid(nullable: false),
                        CompletedOn = c.DateTime(),
                        StartedOn = c.DateTime(nullable: false),
                        Domain = c.String(maxLength: 50),
                        WorkflowParentDefinitionId = c.Int(),
                        WorkflowCodeId = c.Int(nullable: false),
                        WorkflowStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowDefinitionId)
                .ForeignKey("dbo.WorkflowCode", t => t.WorkflowCodeId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowDefinition", t => t.WorkflowParentDefinitionId)
                .ForeignKey("dbo.WorkflowStatus", t => t.WorkflowStatusId, cascadeDelete: true)
                .Index(t => t.WorkflowParentDefinitionId)
                .Index(t => t.WorkflowCodeId)
                .Index(t => t.WorkflowStatusId);
            
            CreateTable(
                "dbo.WorkflowStatus",
                c => new
                    {
                        WorkflowStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.WorkflowStatusId);
            
            CreateTable(
                "dbo.TaskInParameter",
                c => new
                    {
                        TaskInParameterId = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        TaskDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskInParameterId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.TaskDefinition", t => t.TaskDefinitionId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.TaskDefinitionId);
            
            CreateTable(
                "dbo.TaskOutParameter",
                c => new
                    {
                        TaskOutParameterId = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        TaskDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskOutParameterId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.TaskDefinition", t => t.TaskDefinitionId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.TaskDefinitionId);
            
            CreateTable(
                "dbo.TaskProperty",
                c => new
                    {
                        TaskPropertyId = c.Int(nullable: false, identity: true),
                        TaskCode = c.String(nullable: false, maxLength: 20),
                        PropertyId = c.Int(nullable: false),
                        WorkflowCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskPropertyId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowCode", t => t.WorkflowCodeId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.WorkflowCodeId);
            
            CreateTable(
                "dbo.TaskUserHandOver",
                c => new
                    {
                        TaskUserHandOverId = c.Int(nullable: false, identity: true),
                        User = c.String(nullable: false, maxLength: 16),
                        InUse = c.Boolean(nullable: false),
                        TaskDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskUserHandOverId)
                .ForeignKey("dbo.TaskDefinition", t => t.TaskDefinitionId, cascadeDelete: true)
                .Index(t => t.TaskDefinitionId);
            
            CreateTable(
                "dbo.TaskUser",
                c => new
                    {
                        TaskUserId = c.Int(nullable: false, identity: true),
                        User = c.String(nullable: false, maxLength: 16),
                        InUse = c.Boolean(nullable: false),
                        TaskDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskUserId)
                .ForeignKey("dbo.TaskDefinition", t => t.TaskDefinitionId, cascadeDelete: true)
                .Index(t => t.TaskDefinitionId);
            
            CreateTable(
                "dbo.TopicAttachment",
                c => new
                    {
                        TopicAttachmentId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 200),
                        OidDocument = c.Guid(nullable: false),
                        TopicMessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TopicAttachmentId)
                .ForeignKey("dbo.TopicMessage", t => t.TopicMessageId, cascadeDelete: true)
                .Index(t => t.TopicMessageId);
            
            CreateTable(
                "dbo.TopicMessage",
                c => new
                    {
                        TopicMessageId = c.Int(nullable: false, identity: true),
                        Message = c.String(maxLength: 500),
                        From = c.String(maxLength: 20),
                        To = c.String(maxLength: 200),
                        When = c.DateTime(nullable: false),
                        IsTopic = c.Boolean(nullable: false),
                        TopicId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TopicMessageId)
                .ForeignKey("dbo.Topic", t => t.TopicId, cascadeDelete: true)
                .Index(t => t.TopicId);
            
            CreateTable(
                "dbo.Topic",
                c => new
                    {
                        TopicId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 200),
                        LastChanged = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TopicId);
            
            CreateTable(
                "dbo.TopicStatus",
                c => new
                    {
                        TopicStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.TopicStatusId);
            
            CreateTable(
                "dbo.TopicUser",
                c => new
                    {
                        TopicUserId = c.Int(nullable: false, identity: true),
                        User = c.String(nullable: false, maxLength: 16),
                        TopicMessageId = c.Int(nullable: false),
                        TopicStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TopicUserId)
                .ForeignKey("dbo.TopicMessage", t => t.TopicMessageId, cascadeDelete: true)
                .ForeignKey("dbo.TopicStatus", t => t.TopicStatusId, cascadeDelete: true)
                .Index(t => t.TopicMessageId)
                .Index(t => t.TopicStatusId);
            
            CreateTable(
                "dbo.TraceEvent",
                c => new
                    {
                        TraceEventId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.TraceEventId);
            
            CreateTable(
                "dbo.WorkflowConfiguration",
                c => new
                    {
                        WorkflowConfigurationId = c.Int(nullable: false, identity: true),
                        ServiceDefinition = c.String(),
                        ServiceUrl = c.String(maxLength: 256),
                        BindingConfiguration = c.String(maxLength: 50),
                        ServiceEndpoint = c.String(maxLength: 50),
                        EffectiveDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(),
                        WorkflowCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowConfigurationId)
                .ForeignKey("dbo.WorkflowCode", t => t.WorkflowCodeId, cascadeDelete: true)
                .Index(t => t.WorkflowCodeId);
            
            CreateTable(
                "dbo.WorkflowInParameter",
                c => new
                    {
                        WorkflowInParameterId = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        WorkflowDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowInParameterId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowDefinition", t => t.WorkflowDefinitionId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.WorkflowDefinitionId);
            
            CreateTable(
                "dbo.WorkflowOutParameter",
                c => new
                    {
                        WorkflowOutParameterId = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        WorkflowDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowOutParameterId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowDefinition", t => t.WorkflowDefinitionId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.WorkflowDefinitionId);
            
            CreateTable(
                "dbo.WorkflowProperty",
                c => new
                    {
                        WorkflowPropertyId = c.Int(nullable: false, identity: true),
                        Domain = c.String(maxLength: 50),
                        WorkflowCodeId = c.Int(nullable: false),
                        PropertyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowPropertyId)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowCode", t => t.WorkflowCodeId, cascadeDelete: true)
                .Index(t => t.WorkflowCodeId)
                .Index(t => t.PropertyId);
            
            CreateTable(
                "dbo.WorkflowTrace",
                c => new
                    {
                        WorkflowTraceId = c.Int(nullable: false, identity: true),
                        When = c.DateTime(nullable: false),
                        User = c.String(maxLength: 16),
                        Action = c.String(maxLength: 20),
                        Result = c.String(maxLength: 20),
                        Code = c.String(maxLength: 20),
                        Message = c.String(maxLength: 500),
                        WorkflowDefinitionId = c.Int(nullable: false),
                        TraceEventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowTraceId)
                .ForeignKey("dbo.TraceEvent", t => t.TraceEventId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowDefinition", t => t.WorkflowDefinitionId, cascadeDelete: true)
                .Index(t => t.WorkflowDefinitionId)
                .Index(t => t.TraceEventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkflowTrace", "WorkflowDefinitionId", "dbo.WorkflowDefinition");
            DropForeignKey("dbo.WorkflowTrace", "TraceEventId", "dbo.TraceEvent");
            DropForeignKey("dbo.WorkflowProperty", "WorkflowCodeId", "dbo.WorkflowCode");
            DropForeignKey("dbo.WorkflowProperty", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.WorkflowOutParameter", "WorkflowDefinitionId", "dbo.WorkflowDefinition");
            DropForeignKey("dbo.WorkflowOutParameter", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.WorkflowInParameter", "WorkflowDefinitionId", "dbo.WorkflowDefinition");
            DropForeignKey("dbo.WorkflowInParameter", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.WorkflowConfiguration", "WorkflowCodeId", "dbo.WorkflowCode");
            DropForeignKey("dbo.TopicUser", "TopicStatusId", "dbo.TopicStatus");
            DropForeignKey("dbo.TopicUser", "TopicMessageId", "dbo.TopicMessage");
            DropForeignKey("dbo.TopicAttachment", "TopicMessageId", "dbo.TopicMessage");
            DropForeignKey("dbo.TopicMessage", "TopicId", "dbo.Topic");
            DropForeignKey("dbo.TaskUser", "TaskDefinitionId", "dbo.TaskDefinition");
            DropForeignKey("dbo.TaskUserHandOver", "TaskDefinitionId", "dbo.TaskDefinition");
            DropForeignKey("dbo.TaskProperty", "WorkflowCodeId", "dbo.WorkflowCode");
            DropForeignKey("dbo.TaskProperty", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.TaskOutParameter", "TaskDefinitionId", "dbo.TaskDefinition");
            DropForeignKey("dbo.TaskOutParameter", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.TaskInParameter", "TaskDefinitionId", "dbo.TaskDefinition");
            DropForeignKey("dbo.TaskInParameter", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.TaskDefinition", "WorkflowDefinitionId", "dbo.WorkflowDefinition");
            DropForeignKey("dbo.WorkflowDefinition", "WorkflowStatusId", "dbo.WorkflowStatus");
            DropForeignKey("dbo.WorkflowDefinition", "WorkflowParentDefinitionId", "dbo.WorkflowDefinition");
            DropForeignKey("dbo.WorkflowDefinition", "WorkflowCodeId", "dbo.WorkflowCode");
            DropForeignKey("dbo.TaskConfiguration", "WorkflowCodeId", "dbo.WorkflowCode");
            DropForeignKey("dbo.SketchConfiguration", "SketchStatusId", "dbo.SketchStatus");
            DropForeignKey("dbo.Holiday", "HolidayTypeId", "dbo.HolidayType");
            DropIndex("dbo.WorkflowTrace", new[] { "TraceEventId" });
            DropIndex("dbo.WorkflowTrace", new[] { "WorkflowDefinitionId" });
            DropIndex("dbo.WorkflowProperty", new[] { "PropertyId" });
            DropIndex("dbo.WorkflowProperty", new[] { "WorkflowCodeId" });
            DropIndex("dbo.WorkflowOutParameter", new[] { "WorkflowDefinitionId" });
            DropIndex("dbo.WorkflowOutParameter", new[] { "PropertyId" });
            DropIndex("dbo.WorkflowInParameter", new[] { "WorkflowDefinitionId" });
            DropIndex("dbo.WorkflowInParameter", new[] { "PropertyId" });
            DropIndex("dbo.WorkflowConfiguration", new[] { "WorkflowCodeId" });
            DropIndex("dbo.TopicUser", new[] { "TopicStatusId" });
            DropIndex("dbo.TopicUser", new[] { "TopicMessageId" });
            DropIndex("dbo.TopicMessage", new[] { "TopicId" });
            DropIndex("dbo.TopicAttachment", new[] { "TopicMessageId" });
            DropIndex("dbo.TaskUser", new[] { "TaskDefinitionId" });
            DropIndex("dbo.TaskUserHandOver", new[] { "TaskDefinitionId" });
            DropIndex("dbo.TaskProperty", new[] { "WorkflowCodeId" });
            DropIndex("dbo.TaskProperty", new[] { "PropertyId" });
            DropIndex("dbo.TaskOutParameter", new[] { "TaskDefinitionId" });
            DropIndex("dbo.TaskOutParameter", new[] { "PropertyId" });
            DropIndex("dbo.TaskInParameter", new[] { "TaskDefinitionId" });
            DropIndex("dbo.TaskInParameter", new[] { "PropertyId" });
            DropIndex("dbo.WorkflowDefinition", new[] { "WorkflowStatusId" });
            DropIndex("dbo.WorkflowDefinition", new[] { "WorkflowCodeId" });
            DropIndex("dbo.WorkflowDefinition", new[] { "WorkflowParentDefinitionId" });
            DropIndex("dbo.TaskDefinition", new[] { "WorkflowDefinitionId" });
            DropIndex("dbo.TaskConfiguration", new[] { "WorkflowCodeId" });
            DropIndex("dbo.SketchConfiguration", new[] { "SketchStatusId" });
            DropIndex("dbo.Holiday", new[] { "HolidayTypeId" });
            DropTable("dbo.WorkflowTrace");
            DropTable("dbo.WorkflowProperty");
            DropTable("dbo.WorkflowOutParameter");
            DropTable("dbo.WorkflowInParameter");
            DropTable("dbo.WorkflowConfiguration");
            DropTable("dbo.TraceEvent");
            DropTable("dbo.TopicUser");
            DropTable("dbo.TopicStatus");
            DropTable("dbo.Topic");
            DropTable("dbo.TopicMessage");
            DropTable("dbo.TopicAttachment");
            DropTable("dbo.TaskUser");
            DropTable("dbo.TaskUserHandOver");
            DropTable("dbo.TaskProperty");
            DropTable("dbo.TaskOutParameter");
            DropTable("dbo.TaskInParameter");
            DropTable("dbo.WorkflowStatus");
            DropTable("dbo.WorkflowDefinition");
            DropTable("dbo.TaskDefinition");
            DropTable("dbo.WorkflowCode");
            DropTable("dbo.TaskConfiguration");
            DropTable("dbo.SketchStatus");
            DropTable("dbo.SketchConfiguration");
            DropTable("dbo.Property");
            DropTable("dbo.HolidayType");
            DropTable("dbo.Holiday");
        }
    }
}
