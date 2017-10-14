using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [TestMethod, TestCategory("Contact Log")]
        public async Task Get_All_Logs()
        {
            var repo = new OwnerRepository();

            var logs = repo.GetLogs();

            Assert.IsNotNull(logs);
            Assert.AreNotEqual(0, logs.Count());
            Assert.IsTrue(logs.All(cp => cp.Parcels.All(px=> px.IsActive)));

            foreach (var l in logs)
            {
                Trace.WriteLine(l.ContactLogId);
            };

            var llogs = logs.SelectMany(l => LogExport.Export(l));

            Assert.IsTrue(logs.Count() <= llogs.Count());

            var lines = llogs.OrderBy(lp => lp.ParcelId).ThenBy(lp => lp.DateAdded).Select(lp => lp.ToString());
            System.IO.File.WriteAllLines(@"d:\junk\logs.csv", lines);
        }

        [TestMethod]
        public async Task Get_All_Contacts()
        {
            var repo = new OwnerRepository();

            var c = repo.GetContacts();
            Assert.AreNotEqual(0, c.Count());
            Assert.IsTrue(c.All(cp => cp.Parcel.IsActive));

            var cc = c.SelectMany(cx => ContactExport.Export(cx));

            Assert.AreNotEqual(0, cc.Count());

            var lines = cc.OrderBy(cx => cx.ParcelId).ThenByDescending(cx => cx.IsPrimary).ThenBy(cx => cx.LastName).Select( ccx => ccx.ToString());
            System.IO.File.WriteAllLines(@"d:\junk\contacts.csv", lines);
        }

        [TestMethod]
        public async Task Get_All_Documents()
        {
            var repo = new OwnerRepository();

            var d = await repo.GetDocs();
            Assert.AreNotEqual(0, d.Count());

            var lines = d.OrderBy(dh => dh.Parcel_ParcelId)
                .Select(dh => $"=\"{dh.Parcel_ParcelId}\",{dh.Title},{dh.SentDate?.Date.ToShortDateString() ?? ""},{dh.ReceivedDate?.Date.ToShortDateString() ?? ""},{dh.DocumentId}");

            System.IO.File.WriteAllLines(@"d:\junk\documents.csv", lines);
        }
    }

    public class ContactExport
    {
        public string ParcelId { get; set; }
        public string PartyName { get; set; }
        public bool IsPrimary { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public string HomePhone { get; set; }
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string Representation { get; set; }

        public static IEnumerable<ContactExport> Export(Ownership op)
        {
            return op.Owner.Contacts.Select( cx => new ContactExport
            {
                ParcelId = op.ParcelId,
                PartyName = op.Owner.PartyName,
                IsPrimary = cx.IsPrimaryContact,
                FirstName = cx.OwnerFirstName,
                LastName = cx.OwnerLastName,
                Email = cx.OwnerEmail,
                CellPhone = cx.OwnerCellPhone,
                HomePhone = cx.OwnerHomePhone,
                StreetAddress = cx.OwnerStreetAddress,
                State = cx.OwnerState,
                ZIP = cx.OwnerZIP,
                Representation = cx.Representation
            });
        }

        public override string ToString() =>
            $"=\"{ParcelId}\",\"{PartyName}\",{IsPrimary},{FirstName},{LastName},{Email},{CellPhone},{HomePhone},\"{StreetAddress}\",{State},{ZIP},{Representation}";
    }

    public class LogExport
    {
        public string ParcelId { get; set; }
        public string ParcelStatusCode { get; set; }
        public string RoeStatusCode { get; set; }
        public string ContactName { get; set; }

        public DateTimeOffset DateAdded { get; set; }
        public string ContactChannel { get; set; }
        public string ProjectPhase { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }

        public string AgentName { get; set; }

        public static IEnumerable<LogExport> Export(ContactLog log)
        {
            return log.Parcels.Where(p => p.IsActive).Select(p => new LogExport
            {
                ParcelId = p.ParcelId,
                ParcelStatusCode = p.ParcelStatusCode,
                RoeStatusCode = p.RoeStatusCode,
                ContactName = p.Owners.FirstOrDefault()?.Owner.PartyName ?? "",
                DateAdded = log.DateAdded,
                ContactChannel = log.ContactChannel,
                ProjectPhase = log.ProjectPhase,
                Title = log.Title,
                Notes = log.Notes,
                AgentName = log.ContactAgent.AgentName
            });
        }

        public override string ToString()
        {
            var n = Notes.Replace('"', '\'');
            return $"=\"{ParcelId}\",{ParcelStatusCode},{RoeStatusCode},\"{ContactName}\",{DateAdded.Date.ToShortDateString()},{ContactChannel},{ProjectPhase},\"{Title}\",\"{n}\",{AgentName}";
        }
    }
}
