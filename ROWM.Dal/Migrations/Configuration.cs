namespace ROWM.Dal.Migrations
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ROWM.Dal.ROWM_Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        /*
         * stop running seed
         * 
        protected override void Seed(ROWM.Dal.ROWM_Context context)
        {
            context.Purposes.AddOrUpdate(
                r => r.PurposeCode,
                new Purpose_Master { PurposeCode = "ROE", Description = "ROE", DisplayOrder = 1, IsActive = true },
                new Purpose_Master { PurposeCode = "Offer", Description = "Offer", DisplayOrder = 2, IsActive = true },
                new Purpose_Master { PurposeCode = "Negotiation", Description = "Negotiation", DisplayOrder = 3, IsActive = true }
            );

            context.Representations.AddOrUpdate(
                r => r.RelationTypeCode,
                new Representation { RelationTypeCode = "Self", Description = "Self", DisplayOrder = 1, IsActive = true },
                new Representation { RelationTypeCode = "Spouse", Description = "Spouse", DisplayOrder = 2, IsActive = true },
                new Representation { RelationTypeCode = "Child", Description = "Child", DisplayOrder = 3, IsActive = true },
                new Representation { RelationTypeCode = "Tenant", Description = "Tenant", DisplayOrder = 4, IsActive = true },
                new Representation { RelationTypeCode = "Attorney", Description = "Attorney", DisplayOrder = 5, IsActive = true },
                new Representation { RelationTypeCode = "Other", Description = "Other", DisplayOrder = 6, IsActive = true }
            );

            context.Channels.AddOrUpdate(
                c => c.ContactTypeCode,
                new Channel_Master { ContactTypeCode = "Email", Description = "Email", DisplayOrder = 1, IsActive = true },
                new Channel_Master { ContactTypeCode = "In-Person", Description = "In-Person", DisplayOrder = 2, IsActive = true },
                new Channel_Master { ContactTypeCode = "Letter", Description = "Letter", DisplayOrder = 3, IsActive = true },
                new Channel_Master { ContactTypeCode = "Note to File", Description = "Note to File", DisplayOrder = 4, IsActive = true },
                new Channel_Master { ContactTypeCode = "Phone Call", Description = "Phone Call", DisplayOrder = 5, IsActive = true },
                new Channel_Master { ContactTypeCode = "Text Message", Description = "Text Message", DisplayOrder = 6, IsActive = true }
            );

            context.ParcelStatus.AddOrUpdate(
                c => c.Code,
                new ParcelStatus_Master { Code = "No_Activity", Description = "No Activity", DomainValue = 0, DisplayOrder = 0, IsActive = true },
                new ParcelStatus_Master { Code = "Owner_Contacted", Description = "Owner Contacted", DomainValue = 1, DisplayOrder = 1, IsActive = true },
                new ParcelStatus_Master { Code = "ROE_Obtained", Description = "ROE Obtained", DomainValue = 2, DisplayOrder = 2, IsActive = true },
                new ParcelStatus_Master { Code = "Offer_Made", Description = "Offer Made", DomainValue = 3, DisplayOrder = 3, IsActive = true },
                new ParcelStatus_Master { Code = "Easement_Signed", Description = "Easement Signed", DomainValue = 4, DisplayOrder = 4, IsActive = true },
                new ParcelStatus_Master { Code = "Compensation_Check_Cut", Description = "Compensation Check Cut", DomainValue = 5, DisplayOrder = 5, IsActive = true },
                new ParcelStatus_Master { Code = "Documents_Recorded", Description = "Documents Recorded", DomainValue = 6, DisplayOrder = 6, IsActive = true },
                new ParcelStatus_Master { Code = "Compensation_Received_by_Owner", Description = "Compensation Received by Owner", DomainValue = 7, DisplayOrder = 7, IsActive = true }
            );

            context.RoeStatus.AddOrUpdate(
                c => c.Code,
                new RoeStatus_Master { Code = "No_Activity", Description = "No Activity", DomainValue = 0, DisplayOrder = 0, IsActive = true },
                new RoeStatus_Master { Code = "ROE_In_Progress", Description = "ROE In Progress", DomainValue = 1, DisplayOrder = 1, IsActive = true },
                new RoeStatus_Master { Code = "ROE_Obtained", Description = "ROE Obtained", DomainValue = 2, DisplayOrder = 2, IsActive = true },
                new RoeStatus_Master { Code = "ROE_with_Conditions", Description = "ROE With Conditions", DomainValue = 3, DisplayOrder = 3, IsActive = true },
                new RoeStatus_Master { Code = "No_Access", Description = "No Access", DomainValue = 4, DisplayOrder = 4, IsActive = true }
            );

            context.Agents.AddOrUpdate(
                a => a.AgentName,
                new Agent
                {
                    AgentName = "Erin Begier",
                    IsActive = true,
                    Created = DateTimeOffset.Now
                },
                new Agent
                {
                    AgentName = "Amy Borders",
                    IsActive = true,
                    Created = DateTimeOffset.Now
                },
                new Agent
                {
                    AgentName = "Stephen Sykes",
                    IsActive = true,
                    Created = DateTimeOffset.Now
                });

            var data = File.ReadAllText(@"c:\ROWM\ROWM.Dal\Sample_Data\parcels.json");

            var parcels = JObject.Parse(data);

            var olist = new List<Owner>();
            var plist = new List<Parcel>();
            var orlist = new List<Ownership_import>();

            foreach (var parcel in parcels["parcels"])
            {
                // parcels
                var p = new Parcel
                {
                    ParcelId = parcel["PID"].Value<string>(),
                    SitusAddress = parcel["Situs_Address"].Value<string>(),
                    Acreage = parcel["Acres"].Value<double>(),
                    InitialEasementOffer = new Compensation(),
                    InitialOptionOffer = new Compensation(),
                    InitialROEOffer = new Compensation(),
                    FinalEasementOffer = new Compensation(),
                    FinalOptionOffer = new Compensation(),
                    FinalROEOffer = new Compensation()
                };

                // owners
                foreach (var owner in parcel["owners"])
                {
                    var party = owner["Party_Name"].Value<string>();

                    orlist.Add(new Ownership_import
                    {
                        PartyName = party,
                        PID = p.ParcelId
                    });

                    if (olist.Any(ox => ox.PartyName.Equals(party, StringComparison.CurrentCultureIgnoreCase)))
                        continue;

                    var cl = new List<ContactInfo>
                    {
                        new ContactInfo
                        {
                               OwnerStreetAddress = owner["Street_Address"].Value<string>(),
                               OwnerCity = owner["City"].Value<string>(),
                               OwnerState = owner["State"].Value<string>(),
                               OwnerZIP = owner["ZIP"].Value<string>()
                        }
                    };

                    olist.Add(new Owner
                    {
                        PartyName = owner["Party_Name"].Value<string>(),
                        Contacts = cl
                    });
                }

                plist.Add(p);
            }

            context.Parcels.AddOrUpdate(
                p => p.ParcelId,
                plist.ToArray());

            context.Owners.AddOrUpdate(
                p => p.PartyName,
                olist.ToArray());

            context.OwnershipWorking.AddOrUpdate(orlist.ToArray());

        }
        *
        */ 


    }
}
