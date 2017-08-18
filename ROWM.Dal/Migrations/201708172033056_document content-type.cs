namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentcontenttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Document", "ContentType", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("ROWM.Document", "ContentType");
        }
    }
}
