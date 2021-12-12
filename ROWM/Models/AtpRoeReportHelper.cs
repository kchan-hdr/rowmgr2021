using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ROWM.Reports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Models
{
    public class AtpRoeReportHelper
    {        
        internal async Task<ReportPayload> Generate(ROWM.Dal.Parcel parcel)
        {
            if (parcel == null)
                throw new ArgumentNullException();


            var names = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var working = new MemoryStream();
            var s = GetTemplateStream();
            await s.CopyToAsync(working);

            var dox = WordprocessingDocument.Open(working, true);
            dox.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);

            var sdtElements = dox.MainDocumentPart.Document.Descendants<SdtElement>();

            // header
            var head = new RoeReportContent(parcel);
            DoMerge<RoeReportContent>(sdtElements, head);

            // logs
            if (parcel.ContactLog.Any())
            {
                var logrow = Filter(sdtElements, "Logs");
                if (logrow == null)
                {
                    Trace.TraceWarning("report template missing 'Logs'");
                }
                else
                {
                    var myLogs = parcel.ContactLog.Where(lx => !lx.IsDeleted).OrderByDescending(lx => lx.DateAdded).Select(lx => new RoeReportContactLog(lx));                 
                    foreach(var log in myLogs)
                    {
                        var rc = (SdtElement) logrow.CloneNode(true);
                        var elems = logrow.Descendants<SdtElement>();                        
                        DoMerge<RoeReportContactLog>(elems, log);

                        if (log.LogId != myLogs.Last().LogId)
                        {
                            logrow = logrow.InsertAfterSelf(rc);
                        }
                    }
                }
            }

            dox.MainDocumentPart.Document.Save();
            dox.Save();
            dox.Close();

            var rt = new ReportPayload
            {
                Content = working.ToArray(),
                Mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Filename = $"{parcel.Assessor_Parcel_Number} Contact Log by Parcel.docx"
            };
            return rt;
        }

        void DoMerge<T>(IEnumerable<SdtElement> sdtElements, T payload)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var e = Filter(sdtElements, prop.Name);
                if (e != null)
                {
                    var v = prop.GetValue(payload)?.ToString() ?? "";
                    Fill(e, v);
                }
            }
        }
        #region helper
        SdtElement Filter(IEnumerable<SdtElement> elements, string n) =>
            elements.FirstOrDefault(ex => ex.SdtProperties?.GetFirstChild<SdtAlias>()?.Val.Value.ToLower() == n.ToLower());

        void Fill(SdtElement elem, string value)
        {
            try
            {
                var txt = elem.Descendants<Text>();
                foreach (var t in txt)
                    t.Text = value;
            }
            catch( Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }
        #endregion
        #region file helper
        static readonly string _TEMPLATE = "ROWM.ReportTemplates.ContactLogByParcel.dotx";
        Stream GetTemplateStream() =>
            System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(_TEMPLATE);
        #endregion
    }

    #region report dto
    public class RoeReportContent
    {
        public string PrjName { get; set; }
        public string ParcelIdentifier { get; set; }
        public string OwnerName { get; set; }
        public string SiteAddress { get; set; }
        public string MailingAddress { get; set; }
        public string ContactInfo { get; set; }

        public IEnumerable<RoeReportContactLog> ContactLogs { get; set; }

        internal RoeReportContent(ROWM.Dal.Parcel p)
        {
            SiteAddress = p.SitusAddress;
            PrjName = string.Join(" | ", p.ParcelAllocations.Select(ax => ax.ProjectPart.Caption));
            ParcelIdentifier = p.Assessor_Parcel_Number;

            var os = p.Ownership.First().Owner;
            OwnerName = os.PartyName;
            MailingAddress = os.OwnerAddress;

            ContactInfo = string.Join(" | ", os.ContactInfo.Select(pc => new ContactInfox(pc).Full));
        }
    }

    public class RoeReportContactLog
    {
        public Guid LogId { get; set; }
        public string ContactDt { get; set; }
        public string Agent { get; set; }
        public string Contact { get; set; }
        public string Channel { get; set; }
        public string Purpose { get; set; }
        public string Notes { get; set; }

        internal RoeReportContactLog(ROWM.Dal.ContactLog log)
        {
            LogId = log.ContactLogId;
            ContactDt = log.DateAdded.LocalDateTime.ToShortDateString();
            Agent = log.Agent.AgentName;
            Contact = string.Join(" | ", log.ContactInfo.Select(cx => new ContactInfox(cx).Name));
            Channel = log.ContactChannel;
            Purpose = log.ProjectPhase;
            Notes = $"{log.Title} | {log.Notes}";
        }
    }

    public class ContactInfox
    {
        public string Name { get; set; }
        public string Full { get; set; }

        internal ContactInfox(ROWM.Dal.ContactInfo info)
        {
            Name = JoinString(" ", info.FirstName, info.LastName);
            Full = JoinString(", ", Name, info.Representation, info.WorkPhone, info.Email, info.StreetAddress);
        }

        string JoinString(string s, params String[] c ) =>
            string.Join(s, c.Where(cx => !string.IsNullOrWhiteSpace(cx)).Select(cx => cx.Trim()));
    }
    #endregion
}
