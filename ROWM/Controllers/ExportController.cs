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

            // to do. inject export engine
            try
            {
                var d = logs.SelectMany(lx => lx.Parcel.Select(p =>
                {
                    var rgi = lx.Landowner_Score?.ToString() ?? "";
                    var l = new ExcelExport.AgentLogExport.AgentLog
                    {
                        agentname = lx.Agent.AgentName,
                        contactchannel = lx.ContactChannel,
                        dateadded = lx.DateAdded,
                        notes = lx.Notes?.TrimEnd(',') ?? "",
                        ownerfirstname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                        ownerlastname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                        parcelid = p.Assessor_Parcel_Number,
                        parcelstatus = p.Parcel_Status.Description,
                        parcelstatuscode = p.ParcelStatusCode,
                        projectphase = lx.ProjectPhase,
                        roestatus = rgi, // p.Roe_Status.Description,
                        roestatuscode = p.RoeStatusCode,
                        title = lx.Title?.TrimEnd(',') ?? ""
                    };
                    return l;
                }));

                var e = new ExcelExport.AgentLogExport(d);
                var bytes = e.Export();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "logs.xlsx");
            }
            catch (Exception)
            {
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
        }

        /// <summary>
        /// support excel only
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpGet("export/documents")]
        public async Task<IActionResult> ExportDocumentg(string f)
        {
            const string DOCUMENT_HEADER = "Parcel Id,Title,Content Type,Date Sent,Date Delivered,Client Tracking Number,Date Received,Date Signed,Check No,Date Recorded,Document ID";

            if ("excel" != f)
                return BadRequest($"not supported export '{f}'");

            var d = await this._repo.GetDocs();
            if (d.Count() <= 0)
                return NoContent();

            // to do. inject export engine
            try
            {
                var data = d.OrderBy(dh => dh.Parcel_ParcelId).Select(dh => new ExcelExport.DocListExport.DocumentList
                {
                    parcelid = dh.Parcel_ParcelId,
                    title=dh.Title,
                    contenttype =dh.ContentType,
                    sentdate=dh.SentDate,
                    delivereddate=dh.DeliveredDate,
                    clienttrackingnumber=dh.ClientTrackingNumber,
                    signeddate = dh.SignedDate,
                    checkno=dh.CheckNo,
                    receiveddate=dh.ReceivedDate 
                });

                var e = new ExcelExport.DocListExport(data);
                var bytes = e.Export();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "documents.xlsx");
            }
            catch (Exception)
            {
                var lines = d.OrderBy(dh => dh.Parcel_ParcelId)
                    .Select(dh => $"=\"{dh.Parcel_ParcelId}\",\"{dh.Title}\",{dh.ContentType},{dh.SentDate?.Date.ToShortDateString() ?? ""},{dh.DeliveredDate?.Date.ToShortDateString() ?? ""},{dh.ClientTrackingNumber},{dh.ReceivedDate?.Date.ToShortDateString() ?? ""},{dh.SignedDate?.Date.ToShortDateString() ?? ""},=\"{dh.CheckNo}\",{dh.DateRecorded?.Date.ToShortDateString() ?? ""},=\"{dh.DocumentId}\"");

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
        }

        [HttpGet("export/roe")]
        public IActionResult ExportRoe(string f)
        {
            if ("excel" != f)
                return BadRequest($"not supported export '{f}'");

            var parcels = this._repo.GetParcels2();

            if (parcels.Count() <= 0)
                return NoContent();

            // to do. inject export engine
            try
            {
                var data = parcels.OrderBy(px => px.Assessor_Parcel_Number).Select(px => {
                    var os = px.Ownership.OrderBy(ox => ox.IsPrimary() ? 1 : 2).FirstOrDefault();
                    var oname = os?.Owner.PartyName?.TrimEnd(',') ?? "";
                    var p = new ExcelExport.RoeListExport.ParcelList
                    {
                        Parcel_ID = px.Assessor_Parcel_Number,
                        Owner = oname,
                        ROE = px.Roe_Status.Description,
                        Date = px.LastModified
                    };
                    return p;
                });
                var e = new ExcelExport.RoeListExport(data);
                var bytes = e.Export();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "roe.xlsx");
            }
            catch (Exception)
            {
                using (var s = new MemoryStream())
                {
                    using (var writer = new StreamWriter(s))
                    {
                        writer.WriteLine("Parcel ID,Owner,ROE Status,Date");

                        foreach (var p in parcels.OrderBy(px => px.Assessor_Parcel_Number))
                        {
                            var os = p.Ownership.OrderBy(ox => ox.IsPrimary() ? 1 : 2).FirstOrDefault();
                            var oname = os?.Owner.PartyName?.TrimEnd(',') ?? "";
                            var row = $"{p.Assessor_Parcel_Number},\"{oname}\",{p.Roe_Status.Description},{p.LastModified.Date.ToShortDateString()}";
                            writer.WriteLine(row);
                        }

                        writer.Close();
                    }

                    return File(s.GetBuffer(), "text/csv", "roe.csv");
                }
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

            var q = from o in contacts
                    group o by o.OwnerId into og
                    select og;

            var cc = q.SelectMany(og => ContactExport2.Export(og));

            if (cc.Count() <= 0)
                return NoContent();

            // to do. inject export engine
            try
            {
                var data = cc.OrderBy(cx => cx.PartyName)
                                            .ThenByDescending(cx => cx.IsPrimary)
                                            .ThenBy(cx => cx.LastName)
                                            .Select(ccx => new ExcelExport.ContactListExport.ContactList
                                            {
                                                partyname = ccx.PartyName,
                                                isprimarycontact = ccx.IsPrimary,
                                                ownerfirstname = ccx.FirstName,
                                                ownerlastname = ccx.LastName,
                                                owneremail = ccx.Email,
                                                ownercellphone = ccx.CellPhone,
                                                ownerhomephone = ccx.HomePhone,
                                                ownerstreetaddress = ccx.StreetAddress,
                                                ownercity = ccx.City,
                                                ownerstate = ccx.State,
                                                ownerzip = ccx.ZIP,
                                                representation = ccx.Representation,
                                                parcelid = string.Join(",", ccx.ParcelId)
                                            });

                var e = new ExcelExport.ContactListExport(data);
                var bytes = e.Export();
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "contacts.xlsx");
            }
            catch (Exception)
            {
                using (var s = new MemoryStream())
                {
                    using (var writer = new StreamWriter(s))
                    {
                        writer.WriteLine(ContactExport2.Header());

                        foreach (var l in cc.OrderBy(cx => cx.PartyName)
                                            .ThenByDescending(cx => cx.IsPrimary)
                                            .ThenBy(cx => cx.LastName)
                                            .Select(ccx => ccx.ToString()))
                            writer.WriteLine(l);

                        writer.Close();
                    }

                    return File(s.GetBuffer(), "text/csv", "contacts.csv");
                }
            }
        }

        /// <summary>
        /// support excel only
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpGet("export/contacts_i")]
        public IActionResult ExportContactByParcel(string f)
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
                return log.Parcel.Where(p => p.IsActive).Select(p => new LogExport
                {
                    ParcelId = p.Assessor_Parcel_Number,
                    ParcelStatusCode = p.ParcelStatusCode,
                    RoeStatusCode = log.Landowner_Score?.ToString() ?? "", // p.RoeStatusCode,
                    ContactName = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                    DateAdded = log.DateAdded,
                    ContactChannel = log.ContactChannel,
                    ProjectPhase = log.ProjectPhase,
                    Title = log.Title?.TrimEnd(',') ?? "",
                    Notes = log.Notes?.TrimEnd(',') ?? "",
                    AgentName = log.Agent.AgentName
                });
            }

            public static string Header() =>
                "Parcel ID,RGI,Contact Name,Date,Channel,Type,Title,Notes,Agent Name";

            public override string ToString()
            {
                var n = Notes.Replace('"', '\'');
                return $"=\"{ParcelId}\",{RoeStatusCode},\"{ContactName}\",{DateAdded.Date.ToShortDateString()},{ContactChannel},{ProjectPhase},\"{Title}\",\"{n}\",\"{AgentName}\"";
            }
        }

        public class ContactExport2
        {
            public string[] ParcelId { get; set; }


            public string PartyName { get; set; }
            public bool IsPrimary { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string CellPhone { get; set; }
            public string HomePhone { get; set; }
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string Representation { get; set; }

            public static IEnumerable<ContactExport2> Export(IGrouping<Guid, Ownership> og)
            {
                var relatedParcels = og.Select(p => p.Parcel.Assessor_Parcel_Number).OrderBy(p => p).ToArray<string>();

                var ox = og.First();
                return ox.Owner.ContactInfo.Select(cx =>  new ContactExport2
                {
                    PartyName = ox.Owner.PartyName?.TrimEnd(',') ?? "",
                    IsPrimary = cx.IsPrimaryContact,
                    FirstName = cx.OwnerFirstName?.TrimEnd(',') ?? "",
                    LastName = cx.OwnerLastName?.TrimEnd(',') ?? "",
                    Email = cx.OwnerEmail?.TrimEnd(',') ?? "",
                    CellPhone = cx.OwnerCellPhone?.TrimEnd(',') ?? "",
                    HomePhone = cx.OwnerHomePhone?.TrimEnd(',') ?? "",
                    StreetAddress = cx.OwnerStreetAddress?.TrimEnd(',') ?? "",
                    City = cx.OwnerCity?.TrimEnd(',') ?? "",
                    State = cx.OwnerState?.TrimEnd(',') ?? "",
                    ZIP = cx.OwnerZIP?.TrimEnd(',') ?? "",
                    Representation = cx.Representation,
                    ParcelId = relatedParcels
                });
            }

            string RelatedParcels =>
                string.Join(",", this.ParcelId.Select(p=> $"=\"{p}\""));

            public static string Header() =>
                "Owner,Is Primary Contact,First Name,Last Name,Email,Cell Phone,Phone,Street Address,City,State,ZIP,Representation";

            public override string ToString() =>
                $"\"{PartyName}\",{IsPrimary},\"{FirstName}\",\"{LastName}\",{Email},{CellPhone},{HomePhone},\"{StreetAddress}\",{City},{State},{ZIP},{Representation},{RelatedParcels}";
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
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string Representation { get; set; }

            public static IEnumerable<ContactExport> Export(Ownership op)
            {
                return op.Owner.ContactInfo.Select(cx => new ContactExport
                {
                    ParcelId = op.Parcel.Assessor_Parcel_Number,
                    PartyName = op.Owner.PartyName?.TrimEnd(',') ?? "",
                    IsPrimary = cx.IsPrimaryContact,
                    FirstName = cx.OwnerFirstName?.TrimEnd(',') ?? "",
                    LastName = cx.OwnerLastName?.TrimEnd(',') ?? "",
                    Email = cx.OwnerEmail?.TrimEnd(',') ?? "",
                    CellPhone = cx.OwnerCellPhone?.TrimEnd(',') ?? "",
                    HomePhone = cx.OwnerHomePhone?.TrimEnd(',') ?? "",
                    StreetAddress = cx.OwnerStreetAddress?.TrimEnd(',') ?? "",
                    City = cx.OwnerCity?.TrimEnd(',') ?? "",
                    State = cx.OwnerState,
                    ZIP = cx.OwnerZIP,
                    Representation = cx.Representation
                });
            }

            public static string Header() =>
                "Parcel ID,Owner,Is Primary Contact,First Name,Last Name,Email,Cell Phone,Phone,Street Address,City,State,ZIP,Representation";

            public override string ToString() =>
                $"=\"{ParcelId}\",\"{PartyName}\",{IsPrimary},\"{FirstName}\",\"{LastName}\",{Email},{CellPhone},{HomePhone},\"{StreetAddress}\",{City},{State},{ZIP},{Representation}";
        }
        #endregion
    }
}