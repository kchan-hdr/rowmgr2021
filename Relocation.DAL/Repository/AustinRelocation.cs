using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.DAL
{
    public partial class ParcelRelocation : IParcelRelocation
    {
        public IEnumerable<IRelocationCase> RelocationCases { get => this.Cases; }

        public IEnumerable<IRelocationCase> AddCase(RelocationCase c)
        {
            this.Cases.Add(c);
            return this.RelocationCases;
        }

        public IRelocationCase AddCase(string displaceeName, RelocationStatus eligibility, DisplaceeType displaceeType, RelocationType reloType)
        {
            var nCase = this.Cases.Count();

            var aCase = new RelocationCase
            {
                DisplaceeName = displaceeName,
                Status = eligibility,
                DisplaceeType = displaceeType,
                RelocationType = reloType,
                RelocationNumber = nCase + 1    // auto assign relocation number
            };

            aCase.History.Add(new RelocationEligibilityActivity
            {
                ActivityDate = DateTimeOffset.UtcNow,
                AgentId = Guid.NewGuid(),
                NewStatus = eligibility,
                OriginalStatus = RelocationStatus.Unk
            });

            this.Cases.Add(aCase);
            this.LastModified = DateTimeOffset.UtcNow;
            this.ModifiedBy = "ROWM ATP addcase";

            return aCase;
        }
    }
}
