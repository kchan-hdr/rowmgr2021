using System;
using System.Collections.Generic;

namespace com.hdr.rowmgr.Relocation
{
    public interface IParcelRelocation
    {
        Guid ParcelId { get; }
        IEnumerable<IRelocationCase> RelocationCases { get; }

        IRelocationCase AddCase(string displaceeName, RelocationStatus eligibility, DisplaceeType displaceeType, RelocationType reloType);
    }
}
