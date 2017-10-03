namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnonactivebit : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Parcel", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ROWM.Parcel", "IsActive");
        }
    }
}
