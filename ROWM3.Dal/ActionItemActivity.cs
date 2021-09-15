using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Action_Item_Activity", Schema = "ROWM")]
    public class ActionItemActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [ForeignKey(nameof(Item))]
        public Guid ActionItemId { get; set; }
        public virtual ActionItem Item { get; set; }

        [StringLength(1000)]
        public string Action { get; set; }
        [StringLength(1000)]
        public string OriginalAction { get; set; }

        public ActionStatus Status { get; set; }
        public ActionStatus? OriginalStatus { get; set; }

        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset? OriginalDueDate { get; set; }

        public Guid Assigned { get; set; }
        public Guid? OriginalAssigned { get; set; }

        [ForeignKey(nameof(UpdateAgent))]
        public Guid? UpdateAgentId { get; set; }
        public virtual Agent UpdateAgent { get; set; }

        public DateTimeOffset ActivityDate { get; set; }

        [StringLength(int.MaxValue)]
        public string Notes { get; set; }
    }
}
