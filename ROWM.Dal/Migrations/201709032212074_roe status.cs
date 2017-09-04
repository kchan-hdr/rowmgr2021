namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class roestatus0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ROWM.Parcel_Status",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 20),
                        DomainValue = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "ROWM.Roe_Status",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 20),
                        DomainValue = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            AddColumn("ROWM.Parcel", "ParcelStatusCode", c => c.String(maxLength: 20));
            AddColumn("ROWM.Parcel", "RoeStatusCode", c => c.String(maxLength: 20));
            CreateIndex("ROWM.Parcel", "ParcelStatusCode");
            CreateIndex("ROWM.Parcel", "RoeStatusCode");
            AddForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status", "Code");
            AddForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status", "Code");
            DropColumn("ROWM.Parcel", "ParcelStatus");
        }
        
        public override void Down()
        {
            AddColumn("ROWM.Parcel", "ParcelStatus", c => c.Int(nullable: false));
            DropForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status");
            DropForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status");
            DropIndex("ROWM.Parcel", new[] { "RoeStatusCode" });
            DropIndex("ROWM.Parcel", new[] { "ParcelStatusCode" });
            DropColumn("ROWM.Parcel", "RoeStatusCode");
            DropColumn("ROWM.Parcel", "ParcelStatusCode");
            DropTable("ROWM.Roe_Status");
            DropTable("ROWM.Parcel_Status");
        }
    }
}
