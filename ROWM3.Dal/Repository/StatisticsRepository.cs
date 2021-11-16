using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class StatisticsRepository : IStatisticsRepository
    {
        #region ctor
        protected readonly ROWM_Context _context;

        public StatisticsRepository(ROWM_Context c)
        {
            _context = c;
            _baseParcels = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseParcels());
            _baseRoes = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseRoes());
            _baseClearances = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseClearances());
        }
        #endregion

        Lazy<IEnumerable<SubTotal>> _baseParcels;
        Lazy<IEnumerable<SubTotal>> _baseRoes;
        Lazy<IEnumerable<SubTotal>> _baseClearances;

        IEnumerable<SubTotal> _baseAccess = new SubTotal[]{
            new SubTotal { Title = "0", Caption = "Unknown", Count = 0 },
            new SubTotal { Title = "2", Caption = "Unlikely", Count = 0 },
            new SubTotal { Title = "1", Caption = "Likely", Count = 0}};

        protected IQueryable<Parcel> ActiveParcels() => _context.Parcel.Where(px => px.IsActive);
        protected virtual IQueryable<Parcel> ActiveParcels(int? part) => ActiveParcels();

        public async Task<(int nParcels, int nOwners)> Snapshot(int? part = null)
        {
            var actives = ActiveParcels(part);
            var np = await actives.CountAsync(px => px.IsActive);

            var owners = actives.SelectMany(px => px.Ownership.Select(ox => ox.OwnerId));
            var no = await owners.Distinct().CountAsync();

            return (np, no);
        }

        public async Task<IEnumerable<SubTotal>> SnapshotParcelStatus(int? part=null)
        {
            /// TODO: this is too restrictive. need to handle both with/without roll-up
            var q1 = await (from p in ActiveParcels(part)
                            select new { p.ParcelId, p.ParcelStatusCode, p.Parcel_Status.ParentStatusCode })
                     .ToArrayAsync();

            var q = from p in q1
                    group p by string.IsNullOrEmpty(p.ParentStatusCode) ? p.ParcelStatusCode : p.ParentStatusCode into psg
                    select new SubTotal { Title = psg.Key, Count = psg.Count() };
            //var q = await (from p in ActiveParcels(part)
            //               group p by p.ParcelStatusCode into psg
            //               select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseParcels.Value
                      join psg in q on b.Title equals psg.Title into matq
                      from sub in matq.DefaultIfEmpty()
                      select new SubTotal{ Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotRoeStatus(int? part=null)
        {
            var q = await (from p in ActiveParcels(part)
                          group p by p.RoeStatusCode into psg
                          select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseRoes.Value
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotClearanceStatus(int? part=null)
        {
            var q = await (from p in ActiveParcels(part)
                           group p by p.RoeStatusCode into psg
                           select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseClearances.Value
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotAccessLikelihood(int? part=null)
        {
            var q = await (from p in ActiveParcels(part)
                           group p by p.Landowner_Score ?? 0 into psg
                           select new SubTotal { Title = psg.Key.ToString(), Count = psg.Count() }).ToArrayAsync();

            return from b in _baseAccess
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, Count = sub?.Count ?? 0 };
        }

        /// <summary>
        /// Get snapshot counts. Parameterized to replace the other snapshots
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SubTotal>> Snapshot(string cat, int? part = null)
        {
            var baseCounts = await MakeBaseCounts(cat);
            if (!baseCounts.Any())
                throw new KeyNotFoundException($"no category {cat}");

            var snap = GetSnapshot(cat, part);

            return from b in baseCounts
                   join pg in snap on b.Title equals pg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public IQueryable<SubTotal> GetSnapshot(string cat, int? part = null)
        {
            IQueryable<IGrouping<string, Parcel>> myQuery;

            switch ( cat )
            {
                case "acquisition":
                    myQuery = ActiveParcels(part).GroupBy(p => p.ParcelStatusCode).DefaultIfEmpty();
                    break;
                case "roe":
                    myQuery = ActiveParcels(part).GroupBy(p => p.RoeStatusCode).DefaultIfEmpty();
                    break;
                case "engagement":
                    myQuery = ActiveParcels(part).GroupBy(p => p.OutreachStatusCode).DefaultIfEmpty();
                    break;
                case "clearance":
                    myQuery = ActiveParcels(part).GroupBy(p => p.RoeStatusCode).DefaultIfEmpty();
                    break;
                default:
                    myQuery = ActiveParcels(part).GroupBy(p => p.ParcelStatusCode).DefaultIfEmpty();
                    break;
            }

            return myQuery.Select(pg => new SubTotal { Title = pg.Key, Count = pg.Count() });
        }

        public async Task<Financials> GetFinancials(int? part = null)
        {
            var p = await ActiveParcels(part).ToArrayAsync();

            return new Financials
            {
                Appraisal = p.Sum(px => px.InitialROEOffer_OfferAmount) ?? 0,
                Settlement = p.Sum(px => px.FinalROEOffer_OfferAmount) ?? 0,
                ApprovedOffer = p.Sum(px => px.InitialOptionOffer_OfferAmount) ?? 0,
                Sales = p.Sum(px => px.FinalOptionOffer_OfferAmount) ?? 0,
                Closing = p.Sum(px => px.InitialEasementOffer_OfferAmount) ?? 0
            };
        }

        #region helper
        private IEnumerable<SubTotal> MakeBaseParcels() => _context.Parcel_Status.Where(px => px.IsActive && (px.ShowInPie??false) && px.Category == "acquisition").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();
        private IEnumerable<SubTotal> MakeBaseRoes() => _context.Parcel_Status.Where(px => px.IsActive && px.Category == "roe").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code , Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();
        private IEnumerable<SubTotal> MakeBaseClearances() => _context.Parcel_Status.Where(px => px.IsActive && px.Category == "clearance").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();

        private async Task<IEnumerable<SubTotal>> MakeBaseCounts(string cat) =>
            await _context.Parcel_Status.Where(px => px.IsActive && px.Category == cat)
                .OrderBy(px => px.DisplayOrder)
                .Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 })
                .ToArrayAsync();
        #endregion
        #region dto
        public class SubTotal
        {
            public string Title { get; set; }
            public string Caption { get; set; }
            public string DomainValue { get; set; }
            public int Count { get; set; }
        }

        public class Financials
        {
            [DisplayName("Appraisal Value")]
            public double Appraisal { get; set; }    // Initial ROE Offer

            [DisplayName("Admin Settlement")]
            public double Settlement { get; set; }   // Final ROE Offer

            [DisplayName("Approved Offer")]
            public double ApprovedOffer { get; set; }    // Initial Option Offer

            [DisplayName("Contract Sales Price")]
            public double Sales { get; set; }    // Final Option Offer

            [DisplayName("Closing Cost")]
            public double Closing { get; set; } // Initial Easement
        }
        #endregion
    }
}
