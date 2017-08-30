namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class compensationtypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Parcel", "InitialROEOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "InitialROEOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "InitialROEOffer_OfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "FinalROEOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "FinalROEOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "FinalROEOffer_OfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "InitialOptionOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "InitialOptionOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "InitialOptionOffer_OfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "FinalOptionOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "FinalOptionOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "FinalOptionOffer_OfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "InitialEasementOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "InitialEasementOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "InitialEasementOffer_OfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "FinalEasementOffer_OfferDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "FinalEasementOffer_OfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "FinalEasementOffer_OfferNotes", c => c.String());
            DropColumn("ROWM.Parcel", "InitialOffer");
            DropColumn("ROWM.Parcel", "InitialOfferAmount");
            DropColumn("ROWM.Parcel", "InitialOfferNotes");
            DropColumn("ROWM.Parcel", "FinalOffer");
            DropColumn("ROWM.Parcel", "FinalOfferAmount");
            DropColumn("ROWM.Parcel", "FinalOfferNotes");
        }
        
        public override void Down()
        {
            AddColumn("ROWM.Parcel", "FinalOfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "FinalOfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "FinalOffer", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("ROWM.Parcel", "InitialOfferNotes", c => c.String());
            AddColumn("ROWM.Parcel", "InitialOfferAmount", c => c.Double(nullable: false));
            AddColumn("ROWM.Parcel", "InitialOffer", c => c.DateTimeOffset(nullable: false, precision: 7));
            DropColumn("ROWM.Parcel", "FinalEasementOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "FinalEasementOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "FinalEasementOffer_OfferDate");
            DropColumn("ROWM.Parcel", "InitialEasementOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "InitialEasementOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "InitialEasementOffer_OfferDate");
            DropColumn("ROWM.Parcel", "FinalOptionOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "FinalOptionOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "FinalOptionOffer_OfferDate");
            DropColumn("ROWM.Parcel", "InitialOptionOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "InitialOptionOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "InitialOptionOffer_OfferDate");
            DropColumn("ROWM.Parcel", "FinalROEOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "FinalROEOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "FinalROEOffer_OfferDate");
            DropColumn("ROWM.Parcel", "InitialROEOffer_OfferNotes");
            DropColumn("ROWM.Parcel", "InitialROEOffer_OfferAmount");
            DropColumn("ROWM.Parcel", "InitialROEOffer_OfferDate");
        }
    }
}
