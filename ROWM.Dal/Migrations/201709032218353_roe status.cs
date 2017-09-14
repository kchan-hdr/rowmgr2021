namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class roestatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status");
            DropForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status");
            DropIndex("ROWM.Parcel", new[] { "ParcelStatusCode" });
            DropIndex("ROWM.Parcel", new[] { "RoeStatusCode" });
            DropPrimaryKey("ROWM.Parcel_Status");
            DropPrimaryKey("ROWM.Roe_Status");
            AlterColumn("ROWM.Parcel", "ParcelStatusCode", c => c.String(maxLength: 40));
            AlterColumn("ROWM.Parcel", "RoeStatusCode", c => c.String(maxLength: 40));
            AlterColumn("ROWM.Parcel_Status", "Code", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("ROWM.Roe_Status", "Code", c => c.String(nullable: false, maxLength: 40));
            AddPrimaryKey("ROWM.Parcel_Status", "Code");
            AddPrimaryKey("ROWM.Roe_Status", "Code");
            CreateIndex("ROWM.Parcel", "ParcelStatusCode");
            CreateIndex("ROWM.Parcel", "RoeStatusCode");
            AddForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status", "Code");
            AddForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status", "Code");
        }
        
        public override void Down()
        {
            DropForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status");
            DropForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status");
            DropIndex("ROWM.Parcel", new[] { "RoeStatusCode" });
            DropIndex("ROWM.Parcel", new[] { "ParcelStatusCode" });
            DropPrimaryKey("ROWM.Roe_Status");
            DropPrimaryKey("ROWM.Parcel_Status");
            AlterColumn("ROWM.Roe_Status", "Code", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("ROWM.Parcel_Status", "Code", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("ROWM.Parcel", "RoeStatusCode", c => c.String(maxLength: 20));
            AlterColumn("ROWM.Parcel", "ParcelStatusCode", c => c.String(maxLength: 20));
            AddPrimaryKey("ROWM.Roe_Status", "Code");
            AddPrimaryKey("ROWM.Parcel_Status", "Code");
            CreateIndex("ROWM.Parcel", "RoeStatusCode");
            CreateIndex("ROWM.Parcel", "ParcelStatusCode");
            AddForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status", "Code");
            AddForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status", "Code");
        }
    }
}
