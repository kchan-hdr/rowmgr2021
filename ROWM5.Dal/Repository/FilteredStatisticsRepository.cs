using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ROWM.Dal.Repository
{
    public class FilteredStatisticsRepository : StatisticsRepository
    {
        public FilteredStatisticsRepository(ROWM_Context c) : base(c) { }

        protected override IQueryable<Parcel> ActiveParcels(int? part)
        {
            if (!part.HasValue || part == 0)
                return ActiveParcels();

            var q = from p in _context.Parcels.AsNoTracking()
                    join pa in _context.ParcelAllocations.AsNoTracking() on p.ParcelId equals pa.ParcelId
                    where pa.ProjectPartId == part && p.IsActive
                    select p;

            return q;
        }
    }
}
