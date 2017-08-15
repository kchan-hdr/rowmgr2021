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

        protected override void Seed(ROWM.Dal.ROWM_Context context)
        {
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

            context.OwnershipWorking.AddOrUpdate( p=> p.PID, orlist.ToArray());

            context.Agents.AddOrUpdate(
                a => a.AgentName,
                new Agent
                {
                    AgentName = "Agent 99",
                    Created = DateTimeOffset.Now
                },
                new Agent
                {
                    AgentName = "Erin",
                    Created = DateTimeOffset.Now
                });
        }
    }
}
