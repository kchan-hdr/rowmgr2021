namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ROWM.Agent",
                c => new
                    {
                        AgentId = c.Guid(nullable: false, identity: true),
                        AgentName = c.String(nullable: false, maxLength: 20),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.AgentId);
            
            CreateTable(
                "ROWM.ContactLog",
                c => new
                    {
                        ContactLogId = c.Guid(nullable: false, identity: true),
                        DateAdded = c.DateTimeOffset(nullable: false, precision: 7),
                        ContactAgentId = c.Guid(nullable: false),
                        ContactChannel = c.Int(nullable: false),
                        ProjectPhase = c.String(),
                        Title = c.String(maxLength: 200),
                        Notes = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        Owner_OwnerId = c.Guid(),
                    })
                .PrimaryKey(t => t.ContactLogId)
                .ForeignKey("ROWM.Agent", t => t.ContactAgentId, cascadeDelete: true)
                .ForeignKey("ROWM.Owner", t => t.Owner_OwnerId)
                .Index(t => t.ContactAgentId)
                .Index(t => t.Owner_OwnerId);
            
            CreateTable(
                "ROWM.ContactInfo",
                c => new
                    {
                        ContactId = c.Guid(nullable: false, identity: true),
                        IsPrimaryContact = c.Boolean(nullable: false),
                        OwnerFirstName = c.String(maxLength: 50),
                        OwnerLastName = c.String(maxLength: 50),
                        OwnerStreetAddress = c.String(maxLength: 400),
                        OwnerCity = c.String(maxLength: 100),
                        OwnerState = c.String(maxLength: 20),
                        OwnerZIP = c.String(maxLength: 10),
                        OwnerEmail = c.String(maxLength: 256),
                        OwnerHomePhone = c.String(),
                        OwnerCellPhone = c.String(),
                        OwnerWorkPhone = c.String(),
                        ContactOwnerId = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("ROWM.Owner", t => t.ContactOwnerId, cascadeDelete: true)
                .Index(t => t.ContactOwnerId);
            
            CreateTable(
                "ROWM.Owner",
                c => new
                    {
                        OwnerId = c.Guid(nullable: false, identity: true),
                        PartyName = c.String(maxLength: 200),
                        OwnerType = c.String(maxLength: 50),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OwnerId);
            
            CreateTable(
                "ROWM.Ownership",
                c => new
                    {
                        OwnershipId = c.Guid(nullable: false, identity: true),
                        ParcelId = c.String(maxLength: 128),
                        OwnerId = c.Guid(nullable: false),
                        Ownership_t = c.Int(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OwnershipId)
                .ForeignKey("ROWM.Owner", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("ROWM.Parcel", t => t.ParcelId)
                .Index(t => t.ParcelId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "ROWM.Parcel",
                c => new
                    {
                        ParcelId = c.String(nullable: false, maxLength: 128),
                        SitusAddress = c.String(maxLength: 800),
                        ParcelStatus = c.Int(nullable: false),
                        Acreage = c.Double(nullable: false),
                        InitialOffer = c.DateTimeOffset(nullable: false, precision: 7),
                        InitialOfferAmount = c.Double(nullable: false),
                        InitialOfferNotes = c.String(),
                        FinalOffer = c.DateTimeOffset(nullable: false, precision: 7),
                        FinalOfferAmount = c.Double(nullable: false),
                        FinalOfferNotes = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ParcelId);
            
            CreateTable(
                "dbo.Ownership_import",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PartyName = c.String(),
                        PID = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParcelContactLogs",
                c => new
                    {
                        Parcel_ParcelId = c.String(nullable: false, maxLength: 128),
                        ContactLog_ContactLogId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Parcel_ParcelId, t.ContactLog_ContactLogId })
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId, cascadeDelete: true)
                .ForeignKey("ROWM.ContactLog", t => t.ContactLog_ContactLogId, cascadeDelete: true)
                .Index(t => t.Parcel_ParcelId)
                .Index(t => t.ContactLog_ContactLogId);
            
            CreateTable(
                "dbo.ContactInfoContactLogs",
                c => new
                    {
                        ContactInfo_ContactId = c.Guid(nullable: false),
                        ContactLog_ContactLogId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContactInfo_ContactId, t.ContactLog_ContactLogId })
                .ForeignKey("ROWM.ContactInfo", t => t.ContactInfo_ContactId, cascadeDelete: true)
                .ForeignKey("ROWM.ContactLog", t => t.ContactLog_ContactLogId, cascadeDelete: true)
                .Index(t => t.ContactInfo_ContactId)
                .Index(t => t.ContactLog_ContactLogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("dbo.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo");
            DropForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel");
            DropForeignKey("dbo.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("dbo.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactLog", "ContactAgentId", "ROWM.Agent");
            DropIndex("dbo.ContactInfoContactLogs", new[] { "ContactLog_ContactLogId" });
            DropIndex("dbo.ContactInfoContactLogs", new[] { "ContactInfo_ContactId" });
            DropIndex("dbo.ParcelContactLogs", new[] { "ContactLog_ContactLogId" });
            DropIndex("dbo.ParcelContactLogs", new[] { "Parcel_ParcelId" });
            DropIndex("ROWM.Ownership", new[] { "OwnerId" });
            DropIndex("ROWM.Ownership", new[] { "ParcelId" });
            DropIndex("ROWM.ContactInfo", new[] { "ContactOwnerId" });
            DropIndex("ROWM.ContactLog", new[] { "Owner_OwnerId" });
            DropIndex("ROWM.ContactLog", new[] { "ContactAgentId" });
            DropTable("dbo.ContactInfoContactLogs");
            DropTable("dbo.ParcelContactLogs");
            DropTable("dbo.Ownership_import");
            DropTable("ROWM.Parcel");
            DropTable("ROWM.Ownership");
            DropTable("ROWM.Owner");
            DropTable("ROWM.ContactInfo");
            DropTable("ROWM.ContactLog");
            DropTable("ROWM.Agent");
        }
    }
}
