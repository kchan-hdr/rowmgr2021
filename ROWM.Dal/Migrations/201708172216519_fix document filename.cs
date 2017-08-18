namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixdocumentfilename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DocumentOwners", newName: "OwnerDocuments");
            DropForeignKey("ROWM.Agent", "Document_DocumentId", "ROWM.Document");
            DropIndex("ROWM.Agent", new[] { "Document_DocumentId" });
            DropPrimaryKey("dbo.OwnerDocuments");
            CreateTable(
                "dbo.DocumentAgents",
                c => new
                    {
                        Document_DocumentId = c.Guid(nullable: false),
                        Agent_AgentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Document_DocumentId, t.Agent_AgentId })
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .ForeignKey("ROWM.Agent", t => t.Agent_AgentId, cascadeDelete: true)
                .Index(t => t.Document_DocumentId)
                .Index(t => t.Agent_AgentId);
            
            AddColumn("ROWM.Document", "SourceFilename", c => c.String(maxLength: 500));
            AddPrimaryKey("dbo.OwnerDocuments", new[] { "Owner_OwnerId", "Document_DocumentId" });
            DropColumn("ROWM.Agent", "Document_DocumentId");
        }
        
        public override void Down()
        {
            AddColumn("ROWM.Agent", "Document_DocumentId", c => c.Guid());
            DropForeignKey("dbo.DocumentAgents", "Agent_AgentId", "ROWM.Agent");
            DropForeignKey("dbo.DocumentAgents", "Document_DocumentId", "ROWM.Document");
            DropIndex("dbo.DocumentAgents", new[] { "Agent_AgentId" });
            DropIndex("dbo.DocumentAgents", new[] { "Document_DocumentId" });
            DropPrimaryKey("dbo.OwnerDocuments");
            DropColumn("ROWM.Document", "SourceFilename");
            DropTable("dbo.DocumentAgents");
            AddPrimaryKey("dbo.OwnerDocuments", new[] { "Document_DocumentId", "Owner_OwnerId" });
            CreateIndex("ROWM.Agent", "Document_DocumentId");
            AddForeignKey("ROWM.Agent", "Document_DocumentId", "ROWM.Document", "DocumentId");
            RenameTable(name: "dbo.OwnerDocuments", newName: "DocumentOwners");
        }
    }
}
