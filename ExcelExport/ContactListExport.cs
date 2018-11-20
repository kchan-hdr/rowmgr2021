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
    public class ContactListExport : Exporter<Object>
    {
        public ContactListExport(IEnumerable<Object> d) : base(d) { }

        public override byte[] Export()
        {
            reportname = "Contacts List";
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
            WriteText(hr, "B", "Owner");
            WriteText(hr, "C", "Primary Contact");
            WriteText(hr, "D", "Contact Firstname");
            WriteText(hr, "E", "Contact Lastname");
            WriteText(hr, "F", "Contact EMail");
            WriteText(hr, "G", "Contact Cell Phone");
            WriteText(hr, "H", "Contact Home Phone");
            WriteText(hr, "I", "Contact Street Address");
            WriteText(hr, "J", "Contact City");
            WriteText(hr, "K", "Contact State");
            WriteText(hr, "L", "Contact ZIP");
            WriteText(hr, "M", "Representation");

            foreach (dynamic c in items)
            {
                var r = InsertRow(row++, d);
                WriteText(r, "A", c.parcelid);
                WriteText(r, "B", c.partyname);
                WriteText(r, "C", c.isprimarycontact ? "YES" : "NO");
                WriteText(r, "D", c.ownerfirstname);
                WriteText(r, "E", c.ownerlastname);
                WriteText(r, "F", c.owneremail);
                WriteText(r, "G", c.ownercellphone);
                WriteText(r, "H", c.ownerhomephone);
                WriteText(r, "I", c.ownerstreetaddress);
                WriteText(r, "J", c.ownercity);
                WriteText(r, "K", c.ownerstate);
                WriteText(r, "L", c.ownerzip);
                WriteText(r, "M", c.representation);
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Contacts" });
            bookPart.Workbook.Save();
        }

        //List<ContactList> Load()
        //{
        //    using (var ctx = new RowmEntities())
        //    {
        //        var q = ctx.ContactList.AsNoTracking()
        //                    .OrderBy(ax => ax.parcelid).ThenByDescending(ax => ax.isprimarycontact).ThenBy(ax=>ax.ownerlastname);

        //        return q.ToList();
        //    }
        //}
    }
}
