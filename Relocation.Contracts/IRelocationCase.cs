using System;
using System.Collections.Generic;

namespace com.hdr.rowmgr.Relocation
{
    public interface IRelocationCase
    {
        Guid RelocationCaseId { get; }

        Guid? AgentId { get; set; }
        int RelocationNumber { get; }
        RelocationStatus Status { get; set; }
        DisplaceeType DisplaceeType { get; set; }
        RelocationType RelocationType { get; set; }

        string DisplaceeName { get; set; }
        Guid? ContactInfoId { get; set; }


        // details
        IEnumerable<IRelocationEligibilityActivity> EligibilityHistory { get; }
        IEnumerable<IRelocationDisplaceeActivity> DisplaceeActivities { get; }

        // APN or Tracking Number. Use in AcqFilenamePrefix
        string ParcelKey { get; set; }

        // derived
        string AcqFilenamePrefix { get; }
    }
}
