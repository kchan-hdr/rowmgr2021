using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelExport
{
    public class EngagementExport : Exporter<ROWM.Dal.OwnerRepository.EngagementDto>
    {
        public EngagementExport(IEnumerable<ROWM.Dal.OwnerRepository.EngagementDto> d) : base(d) { }

        public override byte[] Export()
        {
            reportname = "Community Engagement Report";

            using (var memory = new MemoryStream())
            {
                var doc = MakeDoc(memory);
                bookPart = doc.AddWorkbookPart();
                bookPart.Workbook = new Workbook();
                sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                MakeStyles();

                WriteEngagement(2);
                WriteAction(3);

                doc.Close();

                return memory.ToArray();
            }
        }

        void WriteEngagement(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            var rx = InsertRow(row++, d);
            WriteText(rx, "A", reportname, 1);
            rx = InsertRow(row++, d);
            WriteText(rx, "A", DateTime.Now.ToLongDateString());

            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "Owner Name", 1);
            WriteText(hr, GetColumnCode(c++), "Project", 1);
            WriteText(hr, GetColumnCode(c++), "Date of Contact", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Name", 1);
            WriteText(hr, GetColumnCode(c++), "Channel", 1);
            WriteText(hr, GetColumnCode(c++), "Type", 1);
            WriteText(hr, GetColumnCode(c++), "Title", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Summary", 1);
            WriteText(hr, GetColumnCode(c++), "Agent Name", 1);

            foreach(var par in items.OrderBy(px => px.TrackingNumber))
            {
                if (par.Logs.Any())
                {
                    foreach (var cot in par.Logs.Where(px => px.ProjectPhase.EndsWith("Engagement")).OrderBy(pdt => pdt.DateAdded))
                    {
                        var r = InsertRow(row++, d);
                        c = 0;
                        WriteText(r, GetColumnCode(c++), par.Apn);
                        WriteText(r, GetColumnCode(c++), string.Join(" | ", par.OwnerName));
                        WriteText(r, GetColumnCode(c++), string.Join(" | ", par.Project));
                        WriteDate(r, GetColumnCode(c++), cot.DateAdded.LocalDateTime);

                        WriteText(r, GetColumnCode(c++), cot.ContactNames);

                        WriteText(r, GetColumnCode(c++), cot.ContactChannel);
                        WriteText(r, GetColumnCode(c++), cot.ProjectPhase);

                        WriteText(r, GetColumnCode(c++), cot.Title);
                        WriteText(r, GetColumnCode(c++), cot.Notes);
                        WriteText(r, GetColumnCode(c++), cot.AgentName);
                    }
                } 
                else
                {
                    var r = InsertRow(row++, d);
                    WriteText(r, GetColumnCode(0), par.Apn);
                    WriteText(r, GetColumnCode(1), string.Join(" | ", par.OwnerName));
                    WriteText(r, GetColumnCode(2), string.Join(" | ", par.Project));
                    WriteText(r, GetColumnCode(6), par.OutreachStatus);
                }
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Outreach" });
            bookPart.Workbook.Save();
        }

        void WriteAction(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            var rx = InsertRow(row++, d);
            WriteText(rx, "A", "Community Engagement Actions", 1);
            rx = InsertRow(row++, d);
            WriteText(rx, "A", DateTime.Now.ToLongDateString());

            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "Owner Name", 1);
            WriteText(hr, GetColumnCode(c++), "Project", 1);
            WriteText(hr, GetColumnCode(c++), "Action Item", 1);
            WriteText(hr, GetColumnCode(c++), "Due Date", 1);
            WriteText(hr, GetColumnCode(c++), "Status", 1);

            foreach (var par in items.OrderBy(px => px.TrackingNumber))
            {
                foreach (var cot in par.Actions.OrderBy(pdt => pdt.Due))
                {
                    var st = Enum.GetName(typeof(ROWM.Dal.ActionStatus), cot.Status);

                    var r = InsertRow(row++, d);
                    c = 0;
                    WriteText(r, GetColumnCode(c++), par.Apn);
                    WriteText(r, GetColumnCode(c++), string.Join(" | ", par.OwnerName));
                    WriteText(r, GetColumnCode(c++), string.Join(" | ", par.Project));
                    WriteText(r, GetColumnCode(c++), cot.Action);
                    WriteDate(r, GetColumnCode(c++), cot.Due.LocalDateTime);
                    WriteText(r, GetColumnCode(c++), st);

                }
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Action Items" });
            bookPart.Workbook.Save();
        }
    }
}
