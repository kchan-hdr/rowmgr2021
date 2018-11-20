using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{
    public class AgentLogExport : Exporter<AgentLogExport.AgentLog>
    {
        public AgentLogExport(IEnumerable<AgentLog> d) : base(d) { }

        public override byte[] Export()
        {
            reportname = "Agent Log";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;
        
            // column heading
            var hr = InsertRow(row++, d);
            WriteText(hr, "A", "Parcel ID");
            WriteText(hr, "B", "Status");
            WriteText(hr, "C", "ROE Status");
            WriteText(hr, "D", "Contact Firstname");
            WriteText(hr, "E", "Contact Lastname");
            WriteText(hr, "F", "Agent Nam");
            WriteText(hr, "G", "Date");
            WriteText(hr, "H", "Channel");
            WriteText(hr, "I", "Type");
            WriteText(hr, "J", "Title");
            WriteText(hr, "K", "Notes");

            foreach (var log in items)
            {
                var r = InsertRow(row++, d);
                WriteText(r, "A", log.parcelid);
                WriteText(r, "B", log.parcelstatus);
                WriteText(r, "C", log.roestatus);
                WriteText(r, "D", log.ownerfirstname);
                WriteText(r, "E", log.ownerlastname);
                WriteText(r, "F", log.agentname);
                WriteText(r, "G", log.dateadded.Date.ToShortDateString());
                WriteText(r, "H", log.contactchannel);
                WriteText(r, "I", log.projectphase);
                WriteText(r, "J", log.title);
                WriteText(r, "K", log.notes);
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Contact Log" });
            bookPart.Workbook.Save();
        }

        #region report dto
        public class AgentLog
        {
            public string parcelid { get; set; }
            public string parcelstatuscode { get; set; }
            public string parcelstatus { get; set; }
            public string roestatuscode { get; set; }
            public string roestatus { get; set; }
            public string ownerfirstname { get; set; }
            public string ownerlastname { get; set; }
            public string agentname { get; set; }
            public System.DateTimeOffset dateadded { get; set; }
            public string contactchannel { get; set; }
            public string projectphase { get; set; }
            public string title { get; set; }
            public string notes { get; set; }
        }
        #endregion
    }
}
