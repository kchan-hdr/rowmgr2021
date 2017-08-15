using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Owner", Schema ="ROWM")]
    public class Owner
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnerId { get; set; }

        [StringLength(200)]
        public string PartyName { get; set; }

        [StringLength(50)]
        public string OwnerType { get; set; }

        // contact info
        public virtual ICollection<ContactInfo> Contacts { get; set; }

        // owned parcel
        public virtual ICollection<Ownership> OwnParcel { get; set; }

        // contact logs
        public virtual ICollection<ContactLog> ContactLogs { get; set; }

        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
