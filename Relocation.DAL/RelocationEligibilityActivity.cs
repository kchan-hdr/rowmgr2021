using com.hdr.rowmgr.Relocation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Relocation_Eligibility", Schema = "Austin")]
    public class RelocationEligibilityActivity : IRelocationEligibilityActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [ForeignKey(nameof(ReloCase))]
        public Guid CaseId { get; set; }
        public virtual RelocationCase ReloCase { get; set; }

        public RelocationStatus OriginalStatus { get; set; }
        public RelocationStatus NewStatus { get; set; }
        public DateTimeOffset ActivityDate { get; set; }

        public Guid AgentId { get; set; }

        [StringLength(400)]
        public string Notes { get; set; }
    }
}
