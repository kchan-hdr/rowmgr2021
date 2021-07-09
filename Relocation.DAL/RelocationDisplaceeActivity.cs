using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Relocation_Displacee_Activity", Schema ="Austin")]
    public class RelocationDisplaceeActivity : IRelocationDisplaceeActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [ForeignKey(nameof(ReloCase))]
        public Guid CaseId { get; set; }
        virtual public RelocationCase ReloCase { get; set; }

        [ForeignKey(nameof(ActivityType))]
        public string ActivityCode { get; set; }
        virtual public RelocationActivityType ActivityType { get; set; }

        public DisplaceeActivity Activity { get; set; }

        /// <summary>
        /// formatted description. just to make db more legitible
        /// </summary>
        [StringLength(int.MaxValue)]
        public string ActivityDescription { get; set; }

        public Guid AgentId { get; set; }

        public DateTimeOffset ActivityDate { get; set; }

        [StringLength(int.MaxValue)]
        public string Notes { get; set; }


        // optional values
        [Column(TypeName ="money")]
        public int? MoneyValue { get; set; }

        public bool? BooleanValue { get; set; }
    }
}
