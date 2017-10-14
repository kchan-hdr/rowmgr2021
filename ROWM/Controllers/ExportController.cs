using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System.IO;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class ExportController : Controller
    {
        OwnerRepository _repo;

        public ExportController(OwnerRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// support excel only
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpGet("export/contactlogs")]
        public IActionResult ExportContactLog(string f)
        {
            if ("excel" != f)
                return BadRequest($"not supported export '{f}'");

            var logs = this._repo.GetLogs();
            if (logs.Count() <= 0)
                return NoContent();

            using (var s = new MemoryStream())
            {
                using (var writer = new StreamWriter(s))
                {
                    writer.WriteLine(LogExport.Header());

                    foreach (var l in logs.SelectMany(l => LogExport.Export(l)))
                        writer.WriteLine(l);

                    writer.Close();
                }
                
                return File(s.GetBuffer(), "text/csv", "logs.csv");
            }
        }

        /// <summary>
        /// support excel only
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpGet("export/documents")]
        public async Task<IActionResult> ExportDocumentg(string f)
        {
            const string DOCUMENT_HEADER = "Parcel Id,Title,Date Sent,Date Received,Document ID";

            if ("excel" != f)
                return BadRequest($"not supported export '{f}'");

            var d = await this._repo.GetDocs();
            if (d.Count() <= 0)
                return NoContent();

            var lines = d.OrderBy(dh => dh.Parcel_ParcelId)
                .Select(dh => $"=\"{dh.Parcel_ParcelId}\",{dh.Title},{dh.SentDate?.Date.ToShortDateString() ?? ""},{dh.ReceivedDate?.Date.ToShortDateString() ?? ""},{dh.DocumentId}");

            using (var s = new MemoryStream())
            {
                using (var writer = new StreamWriter(s))
                {
                    writer.WriteLine(DOCUMENT_HEADER);

                    foreach (var l in lines)
                        writer.WriteLine(l);

                    writer.Close();
                }

                return File(s.GetBuffer(), "text/csv", "documents.csv");
            }
        }

        /// <summary>
        /// support excel only
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpGet("export/contacts")]
        public IActionResult ExportContact(string f)
        {
            if ("excel" != f)
                return BadRequest($"not supported export '{f}'");

            var contacts = this._repo.GetContacts();
            var cc = contacts.SelectMany(cx => ContactExport.Export(cx));
            if (cc.Count() <= 0)
                return NoContent();

            using (var s = new MemoryStream())
            {
                using (var writer = new StreamWriter(s))
                {
                    writer.WriteLine(ContactExport.Header());

                    foreach (var l in cc.OrderBy(cx => cx.ParcelId)
                                        .ThenByDescending(cx => cx.IsPrimary)
                                        .ThenBy(cx => cx.LastName)
                                        .Select(ccx => ccx.ToString()))
                        writer.WriteLine(l);

                    writer.Close();
                }

                return File(s.GetBuffer(), "text/csv", "contacts.csv");
            }
        }
        #region helpers
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

            public static string Header() =>
                "Parcel ID,Parcel Status,ROE Status,Contact Name,Date,Channel,Type,Title,Notes,Agent Name";

            public override string ToString()
            {
                var n = Notes.Replace('"', '\'');
                return $"=\"{ParcelId}\",{ParcelStatusCode},{RoeStatusCode},\"{ContactName}\",{DateAdded.Date.ToShortDateString()},{ContactChannel},{ProjectPhase},\"{Title}\",\"{n}\",{AgentName}";
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
                return op.Owner.Contacts.Select(cx => new ContactExport
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

            public static string Header() =>
                "Parcel ID,Owner,Is Primary Contact,First Name,Last Name,Email,Cell Phone,Phone,Street Address,State,ZIP,Representation";

            public override string ToString() =>
                $"=\"{ParcelId}\",\"{PartyName}\",{IsPrimary},{FirstName},{LastName},{Email},{CellPhone},{HomePhone},\"{StreetAddress}\",{State},{ZIP},{Representation}";
        }
        #endregion
    }
}