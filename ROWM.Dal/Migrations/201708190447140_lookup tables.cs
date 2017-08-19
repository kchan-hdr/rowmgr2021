namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lookuptables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ROWM.Contact_Channel",
                c => new
                    {
                        ContactTypeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ContactTypeCode);
            
            CreateTable(
                "ROWM.Contact_Purpose",
                c => new
                    {
                        PurposeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PurposeCode);
            
            CreateTable(
                "ROWM.Repesentation_Type",
                c => new
                    {
                        RelationTypeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RelationTypeCode);
            
            AddColumn("ROWM.Agent", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("ROWM.ContactInfo", "Representation", c => c.String(maxLength: 20));
            AlterColumn("ROWM.ContactLog", "ContactChannel", c => c.String(maxLength: 20));
            AlterColumn("ROWM.ContactLog", "ProjectPhase", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("ROWM.ContactLog", "ProjectPhase", c => c.String());
            AlterColumn("ROWM.ContactLog", "ContactChannel", c => c.Int(nullable: false));
            DropColumn("ROWM.ContactInfo", "Representation");
            DropColumn("ROWM.Agent", "IsActive");
            DropTable("ROWM.Repesentation_Type");
            DropTable("ROWM.Contact_Purpose");
            DropTable("ROWM.Contact_Channel");
        }
    }
}
