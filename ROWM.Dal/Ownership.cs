using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Ownership", Schema = "ROWM")]
    public class Ownership
    {
        public enum OwnershipType { Primary = 1, Related };

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnershipId { get; set; }

        [ForeignKey("Parcel")]
        public string ParcelId { get; set; }
        public virtual Parcel Parcel { get; set; }

        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public virtual Owner Owner { get; set; }

        public OwnershipType Ownership_t { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
