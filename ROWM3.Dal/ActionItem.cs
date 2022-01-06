using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Action_Item", Schema = "ROWM")]
    public class ActionItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActionItemId { get; set; }

        [ForeignKey(nameof(ParentParcel))]
        public Guid? ParcelId { get; set; }
        public virtual Parcel ParentParcel { get; set; }

        [ForeignKey(nameof(AssignedGroup))]
        public Guid? AssignedGroupId { get; set; }
        public virtual ActionItemGroup AssignedGroup { set; get; }

        [ForeignKey(nameof(ParentActivity))]
        public Guid? ActivityId { get; set; }
        public virtual StatusActivity ParentActivity { get; set; }

        [StringLength(1000)]
        public string Action { get; set; }
        public DateTimeOffset? DueDate { get; set; }

        public ActionStatus Status { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";

        //
        public virtual ICollection<ActionItemActivity> Activities { get; set; } = new HashSet<ActionItemActivity>();
    }

    public enum ActionStatus { Pending = 1, Started, Completed, Canceled }
}
