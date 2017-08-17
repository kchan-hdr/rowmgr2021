namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentsxref : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ROWM.Owner", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Parcel", "Document_DocumentId", "ROWM.Document");
            DropIndex("ROWM.Owner", new[] { "Document_DocumentId" });
            DropIndex("ROWM.Parcel", new[] { "Document_DocumentId" });
            CreateTable(
                "dbo.DocumentOwners",
                c => new
                    {
                        Document_DocumentId = c.Guid(nullable: false),
                        Owner_OwnerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Document_DocumentId, t.Owner_OwnerId })
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .ForeignKey("ROWM.Owner", t => t.Owner_OwnerId, cascadeDelete: true)
                .Index(t => t.Document_DocumentId)
                .Index(t => t.Owner_OwnerId);
            
            CreateTable(
                "dbo.ParcelDocuments",
                c => new
                    {
                        Parcel_ParcelId = c.String(nullable: false, maxLength: 128),
                        Document_DocumentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Parcel_ParcelId, t.Document_DocumentId })
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId, cascadeDelete: true)
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .Index(t => t.Parcel_ParcelId)
                .Index(t => t.Document_DocumentId);
            
            DropColumn("ROWM.Owner", "Document_DocumentId");
            DropColumn("ROWM.Parcel", "Document_DocumentId");
        }
        
        public override void Down()
        {
            AddColumn("ROWM.Parcel", "Document_DocumentId", c => c.Guid());
            AddColumn("ROWM.Owner", "Document_DocumentId", c => c.Guid());
            DropForeignKey("dbo.ParcelDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("dbo.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("dbo.DocumentOwners", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("dbo.DocumentOwners", "Document_DocumentId", "ROWM.Document");
            DropIndex("dbo.ParcelDocuments", new[] { "Document_DocumentId" });
            DropIndex("dbo.ParcelDocuments", new[] { "Parcel_ParcelId" });
            DropIndex("dbo.DocumentOwners", new[] { "Owner_OwnerId" });
            DropIndex("dbo.DocumentOwners", new[] { "Document_DocumentId" });
            DropTable("dbo.ParcelDocuments");
            DropTable("dbo.DocumentOwners");
            CreateIndex("ROWM.Parcel", "Document_DocumentId");
            CreateIndex("ROWM.Owner", "Document_DocumentId");
            AddForeignKey("ROWM.Parcel", "Document_DocumentId", "ROWM.Document", "DocumentId");
            AddForeignKey("ROWM.Owner", "Document_DocumentId", "ROWM.Document", "DocumentId");
        }
    }
}
