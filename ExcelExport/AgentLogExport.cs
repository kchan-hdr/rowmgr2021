using com.hdr.Rowm.Export;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{
    public class AgentLogExport : Exporter
    {
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

            foreach (var log in Load())
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

        List<AgentLog> Load()
        {
            using (var ctx = new RowmEntities())
            {
                var q = ctx.AgentLog.AsNoTracking()
                            .OrderBy(ax => ax.parcelid).ThenBy(ax => ax.dateadded);

                return q.ToList();
            }
        }
    }
}
