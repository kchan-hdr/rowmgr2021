using System;
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

        public StatisticsRepository(ROWM_Context c) => _context = c ?? new ROWM_Context();
        #endregion

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
                           group p by p.Parcel_Status into psg
                           select new { k = psg.Key, c = psg.Count() }).ToArrayAsync();

            var list = new List<SubTotal>();
            foreach (var psg in q)
            {
                list.Add(new SubTotal { Title = psg.k.Code, Count = psg.c });
            }

            return list;
        }

        public async Task<IEnumerable<SubTotal>> SnapshotRoeStatus()
        {
            return await (from p in ActiveParcels()
                          group p by p.RoeStatusCode into psg
                          select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();
        }

        #region dto
        public class SubTotal
        {
            public string Title { get; set; }
            public int Count { get; set; }
        }
        #endregion
    }
}
