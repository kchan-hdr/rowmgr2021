using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Reports
{
    public class AustinReport : IRowmReports
    {
        readonly ROWM_Context _context;
        readonly OwnerRepository _repo;
        public AustinReport(ROWM_Context c, OwnerRepository r) => (_context, _repo) = (c, r);

        public async Task<ReportPayload> GenerateReport(ReportDef d)
        {
            _ = d ?? throw new ArgumentNullException("missing ReportDef");

            var code = d.ReportCode;

            if (code.StartsWith("en") && int.TryParse(code.Substring(2), out var project))
            {
                var data = await _repo.GetEngagement(project);
                var e = new ExcelExport.EngagementExport(data);
                var bytes = e.Export();
                var p = new ReportPayload { Content = bytes, Filename = $"{d.Caption}.xlsx", Mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
                return p;
            }
                

            throw new KeyNotFoundException($"unknown report {code}");
        }

        public IEnumerable<ReportDef> GetReports()
        {
            var parts = _context.ProjectParts
                .Where(pp => pp.IsActive)
                .OrderBy(pp => pp.DisplayOrder)
                .AsEnumerable()
                .Select(pp => new ReportDef { Caption = $"Engagement Report ({pp.Caption})", DisplayOrder = pp.DisplayOrder ?? 0, ReportCode = $"en{pp.ProjectPartId}" })
                .ToArray();

            return parts;
        }
    }
}
