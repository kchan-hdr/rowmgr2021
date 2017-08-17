namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ROWM.DocumentActivity",
                c => new
                    {
                        ActivityId = c.Guid(nullable: false, identity: true),
                        ActivityDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Activity = c.String(nullable: false, maxLength: 100),
                        ActivityNotes = c.String(),
                        ParentDocumentId = c.Guid(nullable: false),
                        ChildDocumentId = c.Guid(),
                        AgentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("ROWM.Agent", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("ROWM.Document", t => t.ChildDocumentId)
                .ForeignKey("ROWM.Document", t => t.ParentDocumentId, cascadeDelete: true)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.ChildDocumentId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "ROWM.Document",
                c => new
                    {
                        DocumentId = c.Guid(nullable: false, identity: true),
                        DocumentType = c.String(maxLength: 50),
                        Content = c.Binary(),
                        SharePointUrl = c.String(),
                        AzureBlobId = c.String(),
                        TitleInFile = c.Boolean(nullable: false),
                        ReceivedDate = c.DateTimeOffset(precision: 7),
                        QCDate = c.DateTimeOffset(precision: 7),
                        ApprovedDate = c.DateTimeOffset(precision: 7),
                        SentDate = c.DateTimeOffset(precision: 7),
                        RowmTrackingNumber = c.String(maxLength: 100),
                        DeliveredDate = c.DateTimeOffset(precision: 7),
                        SignedDate = c.DateTimeOffset(precision: 7),
                        ReceivedFromOwnerDate = c.DateTimeOffset(precision: 7),
                        ClientTrackingNumber = c.String(maxLength: 100),
                        ClientSignatureDate = c.DateTimeOffset(precision: 7),
                        ReceivedFromClientDate = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        DocumentPackage_PackageId = c.Guid(),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("ROWM.DocumentPackage", t => t.DocumentPackage_PackageId)
                .Index(t => t.DocumentPackage_PackageId);
            
            CreateTable(
                "ROWM.DocumentPackage",
                c => new
                    {
                        PackageId = c.Guid(nullable: false, identity: true),
                        PackageName = c.String(nullable: false, maxLength: 100),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PackageId);
            
            AddColumn("ROWM.Agent", "Document_DocumentId", c => c.Guid());
            AddColumn("ROWM.Owner", "Document_DocumentId", c => c.Guid());
            AddColumn("ROWM.Parcel", "Document_DocumentId", c => c.Guid());
            CreateIndex("ROWM.Agent", "Document_DocumentId");
            CreateIndex("ROWM.Owner", "Document_DocumentId");
            CreateIndex("ROWM.Parcel", "Document_DocumentId");
            AddForeignKey("ROWM.Agent", "Document_DocumentId", "ROWM.Document", "DocumentId");
            AddForeignKey("ROWM.Owner", "Document_DocumentId", "ROWM.Document", "DocumentId");
            AddForeignKey("ROWM.Parcel", "Document_DocumentId", "ROWM.Document", "DocumentId");
        }
        
        public override void Down()
        {
            DropForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage");
            DropForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Parcel", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Owner", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Agent", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "AgentId", "ROWM.Agent");
            DropIndex("ROWM.Document", new[] { "DocumentPackage_PackageId" });
            DropIndex("ROWM.DocumentActivity", new[] { "AgentId" });
            DropIndex("ROWM.DocumentActivity", new[] { "ChildDocumentId" });
            DropIndex("ROWM.DocumentActivity", new[] { "ParentDocumentId" });
            DropIndex("ROWM.Parcel", new[] { "Document_DocumentId" });
            DropIndex("ROWM.Owner", new[] { "Document_DocumentId" });
            DropIndex("ROWM.Agent", new[] { "Document_DocumentId" });
            DropColumn("ROWM.Parcel", "Document_DocumentId");
            DropColumn("ROWM.Owner", "Document_DocumentId");
            DropColumn("ROWM.Agent", "Document_DocumentId");
            DropTable("ROWM.DocumentPackage");
            DropTable("ROWM.Document");
            DropTable("ROWM.DocumentActivity");
        }
    }
}
