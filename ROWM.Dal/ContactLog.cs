using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("ContactLog", Schema ="ROWM")]
    public class ContactLog
    {
        public enum Channel { InPerson = 1, Phone, Email, Written, Followup = 10, Research = 20 }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactLogId { get; set; }

        public DateTimeOffset DateAdded { get; set; }

        [ForeignKey("ContactAgent")]
        public Guid ContactAgentId { get; set; }
        public Agent ContactAgent { get; set; }

        public Channel ContactChannel { get; set; }

        public string ProjectPhase { get; set; }

        [StringLength(int.MaxValue)]
        public string Notes { get; set; }

        //
        public virtual ICollection<Owner> Owners { get; set; }
        public virtual ICollection<Parcel> Parcels { get; set; }

        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
