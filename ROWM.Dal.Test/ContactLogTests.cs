using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class ContactLogTests
    {
        [TestMethod]
        public async Task Test_Add_Log()
        {
            var repo = new OwnerRepository();

            var pid = repo.GetParcels().Last();
            var p = await repo.GetParcel(pid);
            var n = p.ContactsLog.Count();

            var a = await repo.GetAgent("Agent 99");
            var px = await repo.AddContactLog(new List<string> { pid }, p.Owners.SelectMany(ox => ox.Owner.Contacts.Select(cx => cx.ContactId)), new ContactLog
            {
                ContactAgent = a,
                Notes = "whatever",
                Title = "whatever",

                Parcels = new List<Parcel>(),
                Contacts = new List<ContactInfo>()
            });

            Assert.IsNotNull(px);

            var pax = px.Parcels.First();
            Assert.AreEqual(pid, pax.ParcelId);
            Assert.AreEqual(n + 1, pax.ContactsLog.Count());
        }

        [TestMethod, TestCategory("Contact Log - Debug")]
        public async Task Debug_Log_Update_Parcel_Link()
        {
            string NewNotes = $"whatever {DateTime.Now}";

            var repo = new OwnerRepository();

            var pid = repo.GetParcels().Last();
            var p = await repo.GetParcel(pid);
            var n = p.ContactsLog.Count();

            var log = p.ContactsLog.FirstOrDefault(l => "whatever".Equals(l.Title));

            log.Notes = NewNotes;
            var px = await repo.UpdateContactLog(null, null, log);

            // check log
            Assert.IsNotNull(px);
            Assert.AreEqual(1, px.Parcels.Count(pax => pax.ParcelId == pid));
            Assert.AreEqual(NewNotes, px.Notes);

            // check parcel again
            var p1 = await repo.GetParcel(pid);
            Assert.AreEqual(1, p1.ContactsLog.Count(l => l.ContactLogId == log.ContactLogId));
        }
    }
}
