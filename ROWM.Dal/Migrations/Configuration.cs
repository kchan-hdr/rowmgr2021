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
            var data = File.ReadAllText(@"D:\Sunflower\ROWM\ROWM.Dal\Sample_Data\parcels.json");

            var parcels = JObject.Parse(data);

            var plist = new List<Parcel>();
            foreach (var parcel in parcels["parcels"])
            {
                var l = new List<Owner>();
                foreach (var owner in parcel["owners"])
                {
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

                    l.Add(new Owner
                    {
                        PartyName = owner["Party_Name"].Value<string>(),
                        Contacts = cl
                    });
                }

                var p = new Parcel
                {
                    ParcelId = parcel["PID"].Value<string>(),
                    SitusAddress = parcel["Situs_Address"].Value<string>(),
                    Acreage = parcel["Acres"].Value<double>(),
                };

                p.Owners = l.Select(lx => new Ownership
                {
                    Owner = lx,
                    Parcel = p,
                    Ownership_t = Ownership.OwnershipType.Primary
                }).ToList();

                plist.Add(p);
            }

            context.Parcels.AddOrUpdate(
                p => p.ParcelId,
                plist.ToArray());


            context.Agents.AddOrUpdate(
                a => a.AgentName,
                new Agent
                {
                    AgentName = "Agent 99",
                    Created = DateTimeOffset.Now
                });
        }
    }
}
