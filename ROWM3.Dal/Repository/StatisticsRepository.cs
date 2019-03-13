﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class StatisticsRepository
    {
        #region ctor
        readonly ROWM_Context _context;

        public StatisticsRepository(ROWM_Context c)
        {
            _context = c;
            _baseParcels = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseParcels());
            _baseRoes = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseRoes());
        }
        #endregion

        Lazy<IEnumerable<SubTotal>> _baseParcels;
        Lazy<IEnumerable<SubTotal>> _baseRoes;

        IEnumerable<SubTotal> _baseAccess = new SubTotal[]{
            new SubTotal { Title = "0", Caption = "Unknown", Count = 0 },
            new SubTotal { Title = "2", Caption = "Unlikely", Count = 0 },
            new SubTotal { Title = "1", Caption = "Likely", Count = 0}};

        IQueryable<Parcel> ActiveParcels() => _context.Parcel.Where(px => px.IsActive);

        public async Task<(int nParcels, int nOwners)> Snapshot()
        {
            var actives = ActiveParcels();
            var np = await actives.CountAsync(px => px.IsActive);

            var owners = actives.SelectMany(px => px.Ownership.Select(ox => ox.OwnerId));
            var no = await owners.Distinct().CountAsync();

            return (np, no);
        }

        public async Task<IEnumerable<SubTotal>> SnapshotParcelStatus()
        {
            var q = await (from p in ActiveParcels()
                           group p by p.ParcelStatusCode into psg
                           select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseParcels.Value
                      join psg in q on b.Title equals psg.Title into matq
                      from sub in matq.DefaultIfEmpty()
                      select new SubTotal{ Title = b.Title, Caption = b.Caption, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotRoeStatus()
        {
            var q = await (from p in ActiveParcels()
                          group p by p.RoeStatusCode into psg
                          select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseRoes.Value
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotAccessLikelihood()
        {
            var q = await (from p in ActiveParcels()
                           group p by p.Landowner_Score ?? 0 into psg
                           select new SubTotal { Title = psg.Key.ToString(), Count = psg.Count() }).ToArrayAsync();

            return from b in _baseAccess
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, Count = sub?.Count ?? 0 };
        }

        #region helper
        private IEnumerable<SubTotal> MakeBaseParcels() => _context.Parcel_Status.Where(px => px.IsActive).OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code, Caption = px.Description, Count = 0 }).ToArray();
        private IEnumerable<SubTotal> MakeBaseRoes() => _context.Roe_Status.Where(px => px.IsActive).OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code , Caption = px.Description, Count = 0 }).ToArray();
        #endregion
        #region dto
        public class SubTotal
        {
            public string Title { get; set; }
            public string Caption { get; set; }
            public int Count { get; set; }
        }
        #endregion
    }
}
