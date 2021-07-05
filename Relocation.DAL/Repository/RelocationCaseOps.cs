using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.DAL
{
    public interface IRelocationCaseOps
    {
        Task<IParcelRelocation> GetRelocation(Guid parcelId);
        Task<IEnumerable<IRelocationCase>> GetRelocationCases(Guid parcelId);
        Task<IParcelRelocation> AddRelocationCase(Guid parcelId, string displaceeName, RelocationStatus eligibility, DisplaceeType displaceeType, RelocationType reloType);
        Task<IParcelRelocation> ChangeEligibility(Guid caseId, RelocationStatus eligibility, Guid agentId, DateTimeOffset date, string notes);

        Task<IParcelRelocation> AddActivity(Guid caseId, string activityCode, DisplaceeActivity act, string desc, Guid agentId, DateTimeOffset date, string notes, int? money = null, bool? bValue = null);

        Task<IEnumerable<IRelocationActivityType>> GetActivityTypes();
    }

    public class RelocationCaseNoOp : IRelocationCaseOps
    {
        public Task<IParcelRelocation> AddActivity(Guid caseId, string activityCode, DisplaceeActivity atc, string desc, Guid agentId, DateTimeOffset date, string notes, int? money = null, bool? bValue = null)
        {
            throw new NotImplementedException();
        }

        public Task<IParcelRelocation> AddRelocationCase(Guid parcelId, string displaceeName, RelocationStatus eligibility, DisplaceeType displaceeType, RelocationType reloType)
        {
            throw new NotImplementedException();
        }

        public Task<IParcelRelocation> ChangeEligibility(Guid caseId, RelocationStatus eligibility, Guid agentId, DateTimeOffset date, string notes)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IRelocationActivityType>> GetActivityTypes()
        {
            throw new NotImplementedException();
        }

        public Task<IParcelRelocation> GetRelocation(Guid parcelId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IRelocationCase>> GetRelocationCases(Guid parcelId)
        {
            throw new NotImplementedException();
        }
    }

    public class RelocationCaseOps : IRelocationCaseOps
    {
        readonly RelocationContext _context;
        readonly RelocationRepository _repo;

        public RelocationCaseOps(RelocationContext c, RelocationRepository r) => (_context, _repo) = (c, r);


        #region case eligibility
        public async Task<IEnumerable<IRelocationActivityType>> GetActivityTypes() => await _repo.GetActivityTypes();

        public async Task<IParcelRelocation> GetRelocation(Guid parcelId) => await _repo.GetRelocation(parcelId);

        public async Task<IEnumerable<IRelocationCase>> GetRelocationCases(Guid parcelId)
        {
            var p = await _repo.GetRelocation(parcelId) ?? throw new KeyNotFoundException(nameof(parcelId));
            return p.RelocationCases;
        }

        public async Task<IParcelRelocation> AddRelocationCase(Guid parcelId, string displaceeName, RelocationStatus eligibility, DisplaceeType displaceeType, RelocationType reloType)
        {
            var p = await _repo.GetRelocation(parcelId) ?? _context.Relocations.Add(new ParcelRelocation { ParcelId = parcelId, Created = DateTimeOffset.UtcNow });
            _ = p.AddCase(displaceeName, eligibility, displaceeType, reloType);
            await _repo.SaveRelocation(p as ParcelRelocation);
            return p;
        }

        public async Task<IParcelRelocation> ChangeEligibility(Guid caseId, RelocationStatus eligibility, Guid agentId, DateTimeOffset date, string notes)
        {
            var c = await _context.RelocationCases.FindAsync(caseId) ?? throw new KeyNotFoundException(nameof(caseId));

            var origin = c.Status;
            c.Status = eligibility;

            c.History.Add(new RelocationEligibilityActivity
            {
                ActivityDate = date,
                AgentId = agentId,
                NewStatus = eligibility,
                OriginalStatus = origin,
                Notes = notes
            });

            var p = c.ParentRelocation;
            p.LastModified = DateTimeOffset.UtcNow;
            p.ModifiedBy = "ROWM ATP change";

            await _repo.SaveRelocation(p);

            return p;
        }
        #endregion

        #region case activity
        public async Task<IParcelRelocation> AddActivity(Guid caseId, string activityCode, DisplaceeActivity act, string desc, Guid agentId, DateTimeOffset date, string notes, int? money, bool? bValue)
        {
            var c = await _context.RelocationCases.FindAsync(caseId) ?? throw new KeyNotFoundException(nameof(caseId));

            c.Activities.Add(new RelocationDisplaceeActivity
            {
                ActivityCode = activityCode,
                Activity = act,
                ActivityDescription = desc,
                ActivityDate = date,
                AgentId = agentId,
                Notes = notes,

                MoneyValue = money,
                BooleanValue = bValue
            });

            var p = c.ParentRelocation;
            p.LastModified = DateTimeOffset.UtcNow;
            p.ModifiedBy = "ROWM ATP tracking";

            await _repo.SaveRelocation(p);

            return p;
        }
        #endregion
    }
}
