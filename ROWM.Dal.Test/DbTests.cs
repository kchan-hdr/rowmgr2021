using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void Simple_Connection_Test()
        {
            var c = new ROWM_Context();
            var n = c.Owners.Count();
            Assert.IsTrue(n >= 0);
        }

        [TestMethod]
        public void Simple_Document_Get()
        {
            var c = new ROWM_Context();
            var d = c.Documents.Find(Guid.Parse("B84A2A09-8A83-E711-80DD-000D3A3645F7"));
            Assert.IsNotNull(d);
            Assert.IsNotNull(d.Content);
        }

        [TestMethod, Ignore]
        public void Seed()
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
                    Acreage = parcel["Acres"].Value<double>()
                };

                p.Owners = l.Select(lx => new Ownership
                {
                    Owner = lx,
                    Parcel = p,
                    Ownership_t = Ownership.OwnershipType.Primary
                }).ToArray();

                plist.Add(p);
            }
        }
    }
}
