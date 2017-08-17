using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("DocumentActivity", Schema = "ROWM")]
    public class DocumentTracking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [Required]
        public DateTimeOffset ActivityDate { get; set; }

        [Required, StringLength(100)]
        public string Activity { get; set; }
        [StringLength(int.MaxValue)]
        public string ActivityNotes { get; set; }
        

        [ForeignKey("ParentDocument")]
        public Guid ParentDocumentId { get; set; }
        public virtual Document ParentDocument { get; set; }

        [ForeignKey("ChildDocument")]
        public Guid? ChildDocumentId { get; set; }
        public virtual Document ChildDocument { get; set; }

        [ForeignKey("Agent")]
        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
    }
}
