using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("ContactInfo", Schema ="ROWM")]
    public class ContactInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactId { get; set; }

        public bool IsPrimaryContact { get; set; }

        [StringLength(50)]
        public string OwnerFirstName { get; set; }
        [StringLength(50)]
        public string OwnerLastName { get; set; }
        [StringLength(400)]
        public string OwnerStreetAddress { get; set; }
        [StringLength(100)]
        public string OwnerCity { get; set; }
        [StringLength(20)]
        public string OwnerState { get; set; }
        [StringLength(10)]
        public string OwnerZIP { get; set; }
        [StringLength(256)]
        public string OwnerEmail { get; set; }
        public string OwnerHomePhone { get; set; }
        public string OwnerCellPhone { get; set; }
        public string OwnerWorkPhone { get; set; }

        [ForeignKey("ContactOwner")]
        public Guid ContactOwnerId { get; set; }
        public virtual Owner ContactOwner { get; set; }

        public ICollection<ContactLog> ContactsLog { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
