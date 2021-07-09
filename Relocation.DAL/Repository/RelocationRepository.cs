using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class RelocationRepository
    {
        #region ctor
        readonly RelocationContext _context;
        public RelocationRepository(RelocationContext c)
        {
            _context = c ?? new RelocationContext("name=ROWM_Context");
        }
        #endregion

        public async Task<IEnumerable<IRelocationActivityType>> GetActivityTypes() =>
            await _context.RelocationActivity_Type
                .Where(t => t.IsActive)
                .ToListAsync();

        public async Task<bool> HasRelocation(Guid parcelId) => await _context.Relocations.AnyAsync(r => r.ParcelId == parcelId);
        public async Task<IParcelRelocation> GetRelocation(Guid parcelId) => 
            await _context.Relocations
                .SingleOrDefaultAsync(r => r.ParcelId == parcelId);


        internal ParcelRelocation MakeNewRelocation => _context.Relocations.Add(new ParcelRelocation());

        internal async Task<IParcelRelocation> SaveRelocation(ParcelRelocation r)
        {
            if (_context.Entry<ParcelRelocation>(r).State == EntityState.Detached)
                _context.Entry<ParcelRelocation>(r).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return r;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                throw;
            }
        }

    }
}
