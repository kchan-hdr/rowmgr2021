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
    public class DocListExport : Exporter<Object>
    {
        public DocListExport(IEnumerable<Object> d) : base(d) { }

        public override byte[] Export()
        {
            reportname = "Documents List";
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
            WriteText(hr, "B", "Title");
            WriteText(hr, "C", "Document Type");
            WriteText(hr, "D", "Date Sent");
            WriteText(hr, "E", "Date Delivered");
            WriteText(hr, "F", "Client Tracking Number");
            WriteText(hr, "G", "Date Received");
            WriteText(hr, "H", "Date Signed");
            WriteText(hr, "I", "Check No");
            WriteText(hr, "J", "Date Recorded");

            foreach (dynamic doc in items)
            {
                var r = InsertRow(row++, d);
                WriteText(r, "A", doc.parcelid);
                WriteText(r, "B", doc.title);
                WriteText(r, "C", doc.contenttype);
                WriteText(r, "D", doc.sentdate.HasValue ? doc.sentdate.Value.Date.ToShortDateString() : "");
                WriteText(r, "E", doc.delivereddate.HasValue ? doc.delivereddate.Value.Date.ToShortDateString() : "");
                WriteText(r, "F", doc.clienttrackingnumber);
                WriteText(r, "G", doc.receiveddate.HasValue ? doc.receiveddate.Value.Date.ToShortDateString() : "");
                WriteText(r, "H", doc.signeddate.HasValue ? doc.signeddate.Value.Date.ToShortDateString() : "");
                WriteText(r, "I", doc.checkno);
                WriteText(r, "J", doc.daterecorded.HasValue ? doc.daterecorded.Value.Date.ToShortDateString() : "");
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Documents" });
            bookPart.Workbook.Save();
        }

        //List<DocumentList> Load()
        //{
        //    using (var ctx = new RowmEntities())
        //    {
        //        var q = ctx.DocumentList.AsNoTracking()
        //                    .OrderBy(ax => ax.parcelid).ThenBy(ax => ax.title);

        //        return q.ToList();
        //    }
        //}
    }
}
