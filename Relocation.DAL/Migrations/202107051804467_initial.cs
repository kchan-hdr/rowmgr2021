namespace Relocation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Austin.Relocation_Displacee_Activity",
                c => new
                    {
                        ActivityId = c.Guid(nullable: false, identity: true),
                        CaseId = c.Guid(nullable: false),
                        ActivityCode = c.String(maxLength: 20),
                        Activity = c.Int(nullable: false),
                        ActivityDescription = c.String(),
                        AgentId = c.Guid(nullable: false),
                        ActivityDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Notes = c.String(),
                        MoneyValue = c.Decimal(storeType: "money"),
                        BooleanValue = c.Boolean(),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("Austin.Relocation_Activity_Type", t => t.ActivityCode)
                .ForeignKey("Austin.Relocation_Case", t => t.CaseId, cascadeDelete: true)
                .Index(t => t.CaseId)
                .Index(t => t.ActivityCode);
            
            CreateTable(
                "Austin.Relocation_Activity_Type",
                c => new
                    {
                        ActivityTypeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TrackSent = c.Boolean(nullable: false),
                        TrackDelivered = c.Boolean(nullable: false),
                        TrackSentConsultant = c.Boolean(nullable: false),
                        TrackSentClient = c.Boolean(nullable: false),
                        TrackClientApproval = c.Boolean(nullable: false),
                        IncludeYesNo = c.Boolean(nullable: false),
                        IncludeMoney = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityTypeCode);
            
            CreateTable(
                "Austin.Relocation_Case",
                c => new
                    {
                        RelocationCaseId = c.Guid(nullable: false, identity: true),
                        ParentRelocationId = c.Guid(nullable: false),
                        AgentId = c.Guid(),
                        RelocationNumber = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DisplaceeType = c.Int(nullable: false),
                        RelocationType = c.Int(nullable: false),
                        DisplaceeName = c.String(maxLength: 200),
                        ContactInfoId = c.Guid(),
                    })
                .PrimaryKey(t => t.RelocationCaseId)
                .ForeignKey("Austin.Parcel_Relocation", t => t.ParentRelocationId, cascadeDelete: true)
                .Index(t => t.ParentRelocationId);
            
            CreateTable(
                "Austin.Relocation_Eligibility",
                c => new
                    {
                        ActivityId = c.Guid(nullable: false, identity: true),
                        CaseId = c.Guid(nullable: false),
                        OriginalStatus = c.Int(nullable: false),
                        NewStatus = c.Int(nullable: false),
                        ActivityDate = c.DateTimeOffset(nullable: false, precision: 7),
                        AgentId = c.Guid(nullable: false),
                        Notes = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("Austin.Relocation_Case", t => t.CaseId, cascadeDelete: true)
                .Index(t => t.CaseId);
            
            CreateTable(
                "Austin.Parcel_Relocation",
                c => new
                    {
                        ParcelRelocationId = c.Guid(nullable: false, identity: true),
                        ParcelId = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ParcelRelocationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Austin.Relocation_Displacee_Activity", "CaseId", "Austin.Relocation_Case");
            DropForeignKey("Austin.Relocation_Case", "ParentRelocationId", "Austin.Parcel_Relocation");
            DropForeignKey("Austin.Relocation_Eligibility", "CaseId", "Austin.Relocation_Case");
            DropForeignKey("Austin.Relocation_Displacee_Activity", "ActivityCode", "Austin.Relocation_Activity_Type");
            DropIndex("Austin.Relocation_Eligibility", new[] { "CaseId" });
            DropIndex("Austin.Relocation_Case", new[] { "ParentRelocationId" });
            DropIndex("Austin.Relocation_Displacee_Activity", new[] { "ActivityCode" });
            DropIndex("Austin.Relocation_Displacee_Activity", new[] { "CaseId" });
            DropTable("Austin.Parcel_Relocation");
            DropTable("Austin.Relocation_Eligibility");
            DropTable("Austin.Relocation_Case");
            DropTable("Austin.Relocation_Activity_Type");
            DropTable("Austin.Relocation_Displacee_Activity");
        }
    }
}
