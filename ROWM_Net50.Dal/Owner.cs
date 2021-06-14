namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Owner", Schema ="ROWM")]
    public partial class Owner
    {
        public Owner()
        {
            ContactInfo = new HashSet<ContactInfo>();
            ContactLog = new HashSet<ContactLog>();
            Ownership = new HashSet<Ownership>();
            Document = new HashSet<Document>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnerId { get; set; }

        [StringLength(200)]
        public string PartyName { get; set; }

        [StringLength(50)]
        public string OwnerType { get; set; }

        public string OwnerAddress { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public virtual ICollection<ContactInfo> ContactInfo { get; set; }

        public virtual ICollection<ContactLog> ContactLog { get; set; }

        public virtual ICollection<Ownership> Ownership { get; set; }

        public virtual ICollection<Document> Document { get; set; }

        public virtual ICollection<Owner> RelatedOwners { get; set; }
    }
}
