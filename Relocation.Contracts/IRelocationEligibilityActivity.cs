using System;

namespace com.hdr.rowmgr.Relocation
{
    public interface IRelocationEligibilityActivity
    {
        Guid CaseId { get; }
        RelocationStatus OriginalStatus { get; }
        RelocationStatus NewStatus { get; }
        DateTimeOffset ActivityDate { get; }
        Guid AgentId { get; }
        string Notes { get; }
    }
}
