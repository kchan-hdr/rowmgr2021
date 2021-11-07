using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [ApiController]
    public class AtcExportController : ControllerBase
    {
        ROWM_Context _ctx;

        public AtcExportController(ROWM_Context c) => _ctx = c;

        [HttpGet("export/atcroetable")]
        public IActionResult ExportAtcRoeTable()
        {
            const string HDR = "Segment,Tract,Owner Name,Contact Name,Contact Phone,No Actvity,ROE In Progress,ROE Obtained - Survey,ROE Obtained - Boring,ROE Obtained Survey and Boring,No Access";

            // summary
            var a = from p in _ctx.Parcels
                    where p.IsActive
                    select p;

            var roes = from status in _ctx.RoeStatuses
                       join t in a on status.Code equals t.RoeStatusCode into g
                       select new { status.Description, status.DisplayOrder, g };

            // details
            var q = _ctx.Database.SqlQuery<StatusTable>("select * from dbo.tract_roe_status_view order by 1");

            using (var s = new MemoryStream())
            {
                using (var writer = new StreamWriter(s))
                {
                    writer.WriteLine($"\"{DateTime.Today.ToLongDateString()}\"");
                    writer.WriteLine($"Total Parcels,{a.Count()}");

                    foreach (var roe in roes.OrderBy(rx => rx.DisplayOrder))
                        writer.WriteLine($"{roe.Description},{roe.g.Count()}");

                    writer.WriteLine(" ");

                    writer.WriteLine(HDR);

                    foreach (var l in q)
                        writer.WriteLine(l);

                    writer.Close();
                }

                return File(s.GetBuffer(), "text/csv", "roe-status.csv");
            }
        }

        [HttpGet("export/atcroepaymenttable")]
        public async Task<IActionResult> ExportAtcRoePaymentTable()
        {
            const string HDR = "Tract,Property Owner,Initial ROE Offer,Final ROE Offer,Primary Contact Name,Primary Contact Phone";

            var pay = _ctx.Parcels.Where(px => px.IsActive);

            var totalInitial = pay.Sum(px => px.InitialRoeofferOfferAmount);
            var totalFinal = pay.Sum(px => px.FinalRoeofferOfferAmount);

            var pays = await pay
                .Where(p => p.InitialRoeofferOfferAmount.HasValue || p.FinalRoeofferOfferAmount.HasValue)
                .Select(p => new { p, p.Ownerships.FirstOrDefault().Owner.PartyName, c = p.Ownerships.FirstOrDefault().Owner.ContactInfos.FirstOrDefault(ct => ct.Representation == "comc") })
                .ToListAsync();

            using (var s = new MemoryStream())
            {
                using (var writer = new StreamWriter(s))
                {
                    writer.WriteLine($"\"{DateTime.Today.ToLongDateString()}\"");
                    writer.WriteLine($"Total Initial ROE Offer,{PrettyMoney(totalInitial)}");
                    writer.WriteLine($"Total Final ROE Offer,{PrettyMoney(totalFinal)}");

                    writer.WriteLine(" ");

                    writer.WriteLine(HDR);

                    foreach (var tract in pays.OrderBy(px => px.p.TrackingNumber))
                        writer.WriteLine($"{tract.p.TrackingNumber},\"{tract.PartyName}\",{PrettyMoney(tract.p.InitialRoeofferOfferAmount)},{PrettyMoney(tract.p.FinalRoeofferOfferAmount)},{tract.c?.FirstName ?? string.Empty},{tract.c?.WorkPhone ?? string.Empty}");

                    writer.Close();
                }

                return File(s.GetBuffer(), "text/csv", "roe-payment.csv");
            }
        }

        #region helper
        static string PrettyMoney(double? pay) =>
            pay.HasValue
                ? $"\"{pay.Value.ToString("C")}\""
                : string.Empty;
        #endregion
    }

    #region dto
    /// <summary>
    /// these are extra "reports" for ATC. dont want to impact the core db context
    /// </summary>
    public class StatusTable
    {
        public string Segment { get; set; }
        public string Notes { get; set; }
        public string Tract { get; set; }
        public string Owner_Name { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Phone { get; set; }
        public int No_Activity { get; set; }
        public int ROE_In_Progress { get; set; }
        public int ROE_Obtained_Survey { get; set; }
        public int ROE_Obtained_Boring { get; set; }
        public int ROE_Obtained_Survey_Boring { get; set; }
        public int No_Access { get; set; }

        public override string ToString() => $"{Segment},{Tract},{PrettyPrint(Owner_Name)},{PrettyPrint(Contact_Name)},{PrettyPrint(Contact_Phone)},{PrettyPrint(No_Activity)},{PrettyPrint(ROE_In_Progress)},{PrettyPrint(ROE_Obtained_Survey)},{PrettyPrint(ROE_Obtained_Boring)},{PrettyPrint(ROE_Obtained_Survey_Boring)},{(PrettyPrint(No_Access))},{PrettyPrint(Notes)}";

        static string PrettyPrint(int i) => i > 0 ? "\"X\"" : string.Empty;
        static string PrettyPrint(string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : $"\"{s.Trim()}\"";
    }
    #endregion
}