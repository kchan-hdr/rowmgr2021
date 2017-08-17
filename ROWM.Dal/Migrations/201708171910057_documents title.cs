namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentstitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Document", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ROWM.Document", "Title");
        }
    }
}
