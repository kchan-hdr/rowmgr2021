using System.Collections.Generic;

namespace com.hdr.rowmgr.Relocation
{
    public interface IRelocationActivityType
    {
        string ActivityTypeCode { get; }

        string Description { get; }
        int DisplayOrder { get; }
        bool IsActive { get; }

        bool TrackSent { get; }
        bool TrackDelivered { get; }
        bool TrackSentConsultant { get; }
        bool TrackSentClient { get; }
        bool TrackClientApproval { get; }

        bool IncludeYesNo { get; }
        bool IncludeMoney { get; }

        IEnumerable<ActivityTaskPick> GetTasks();
    }

    /// <summary>
    /// for picklist
    /// </summary>
    public struct ActivityTaskPick
    {
        public string ActivityTypeCode { get; set; }
        public DisplaceeActivity Activity { get; set; }
        public string Caption { get; set; }
    }
}
