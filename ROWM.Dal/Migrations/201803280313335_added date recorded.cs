namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeddaterecorded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Document", "DateRecorded", c => c.DateTimeOffset(precision: 7));
            AddColumn("ROWM.Document", "CheckNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ROWM.Document", "CheckNo");
            DropColumn("ROWM.Document", "DateRecorded");
        }
    }
}
