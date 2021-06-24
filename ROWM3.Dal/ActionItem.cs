using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Action_Item", Schema = "ROWM")]
    public class ActionItem
    {
        [Key]
        public Guid ActionItemId { get; set; }

        [ForeignKey(nameof(ParentActivity))]
        public Guid ActivityId { get; set; }
        public StatusActivity ParentActivity { get; set; }

        public string Action { get; set; }
        public DateTimeOffset DueDate { get; set; }

        public ActionStatus Status { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }

    public enum ActionStatus { Created = 1, Started, Completed, Canceled }
}
