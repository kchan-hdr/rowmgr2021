﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{   using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using System.IO;

    /// <summary>
    /// output formatted excel for b2h
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Exporter<T>
    {
        protected IEnumerable<T> items;
        protected string reportname = "Unknown";
        protected WorkbookPart bookPart;
        protected Sheets sheets;

        public Exporter(IEnumerable<T> data) {
            items = data;
        }

        virtual public byte[] Export()
        {
            using (var memory = new MemoryStream())
            {
                var doc = MakeDoc(memory);
                bookPart = doc.AddWorkbookPart();
                bookPart.Workbook = new Workbook();
                sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                WriteCoverPage(1, reportname);
                Write(2);

                doc.Close();

                return memory.ToArray();
            }

            //var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            //path = Path.ChangeExtension(path, "xlsx");
            //var doc = MakeDoc(path);
            //bookPart = doc.AddWorkbookPart();
            //bookPart.Workbook = new Workbook();
            //sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            //WriteCoverPage(1, reportname);
            //Write(2);

            //doc.Close();
            //var b = File.ReadAllBytes(path);
            //File.Delete(path);
            //return b;
        }
        virtual protected void Write(uint pageId) { }

        #region implementation
        void WriteCoverPage(uint id, string name)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{id}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint rowId = 1;
            var r = InsertRow(rowId, d);
            WriteText(r, "A", name);
            WriteText(r, "B", DateTime.Now.ToLongDateString());
            WriteText(r, "C", DateTime.Now.ToLongTimeString());

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = id, Name = name });
            bookPart.Workbook.Save();
        }
        #endregion

        #region helpers
        static SpreadsheetDocument MakeDoc(Stream s) => SpreadsheetDocument.Create(s, SpreadsheetDocumentType.Workbook);

        static SpreadsheetDocument MakeDoc(string path) => SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);

        static protected Cell WriteNumber(Row row, string c, string text) => WriteCell(row, c, text, CellValues.Number);
        static protected Cell WriteTrueFalse(Row row, string c, string text) => WriteCell(row, c, text, CellValues.Boolean);
        static protected Cell WriteText(Row row, string c, string text) => WriteCell(row, c, text, CellValues.String);

        static protected Cell WriteCell(Row row, string c, string text, CellValues cell_t)
        {
            var cell = InsertCell(row, c);
            cell.DataType = cell_t;
            cell.CellValue = new CellValue(text);
            return cell;
        }
        static protected Cell InsertCell(Row row, string c)
        {
            var myRange = $"{c}{row.RowIndex}";
            Cell cell;
            if (row.Elements<Cell>().Count(cx => cx.CellReference.Value == myRange) > 0)
            {
                cell = row.Elements<Cell>().First(cx => cx.CellReference.Value == myRange);
            }
            else
            {
                Cell refc = null;
                foreach (var cx in row.Elements<Cell>())
                {
                    if (cx.CellReference.Value.Length == myRange.Length)
                    {
                        if (string.Compare(cx.CellReference.Value, myRange, true) > 0)
                        {
                            refc = cx;
                            break;
                        }
                    }
                }

                cell = new Cell { CellReference = myRange };
                row.InsertBefore(cell, refc);
            }
            return cell;
        }

        static protected Row InsertRow(uint r, SheetData d)
        {
            Row row;
            if ( d.Elements<Row>().Where( rx=> rx.RowIndex==r).Count() <= 0)
            {
                row = new Row() { RowIndex = r };
                d.Append(row);
            }
            else
            {
                row = d.Elements<Row>().First(rx => rx.RowIndex == r);
            }
            return row;
        }
        #endregion
    }
}
