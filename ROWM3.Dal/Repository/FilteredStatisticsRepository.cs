using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Repository
{
    public class FilteredStatisticsRepository : StatisticsRepository
    {
        public FilteredStatisticsRepository(ROWM_Context c) : base(c) { }

        protected override IQueryable<Parcel> ActiveParcels(int? part)
        {
            if (!part.HasValue)
                return ActiveParcels();

            var q = from p in _context.Parcel.AsNoTracking()
                    join pa in _context.Allocations.AsNoTracking() on p.ParcelId equals pa.ParcelId
                    where pa.ProjectPartId == part && p.IsActive
                    select p;

            return q;
        }
    }
}
