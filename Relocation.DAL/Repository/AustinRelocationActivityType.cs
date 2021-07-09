using com.hdr.rowmgr.Relocation;
using System.Collections.Generic;
using System.Linq;

namespace ROWM.Dal
{
    public partial class RelocationActivityType : IRelocationActivityType
    {
        public IEnumerable<ActivityTaskPick> GetTasks()
        {
            var trackingTasks = new List<ActivityTaskPick>();

            if (this.TrackSent)
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.sent, ActivityTypeCode = this.ActivityTypeCode, Caption = $"{this.Description} Sent" });

            if (this.TrackDelivered)
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.delivered, ActivityTypeCode = this.ActivityTypeCode, Caption = $"{this.Description} Delivered" });

            if (this.TrackSentConsultant)
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.toConsultant, ActivityTypeCode = this.ActivityTypeCode, Caption = $"{this.Description} Submitted to Consultant" });

            if (this.TrackSentClient)
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.toClient, ActivityTypeCode = this.ActivityTypeCode, Caption = $"{this.Description} Submitted to Client" });
            
            if (this.TrackClientApproval)
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.clientApproved, ActivityTypeCode = this.ActivityTypeCode, Caption = $"{this.Description} Approved by Client" });

            if (!trackingTasks.Any())
                trackingTasks.Add(new ActivityTaskPick { Activity = DisplaceeActivity.unk, ActivityTypeCode = this.ActivityTypeCode, Caption = this.Description });

            return trackingTasks;
        }
    }
}
