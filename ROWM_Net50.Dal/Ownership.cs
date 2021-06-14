namespace ROWM.Dal
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Ownership", Schema ="ROWM")]
    public partial class Ownership
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnershipId { get; set; }

        public Guid? ParcelId { get; set; }

        public Guid OwnerId { get; set; }

        public int Ownership_t { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public virtual Owner Owner { get; set; }

        public virtual Parcel Parcel { get; set; }
    }
}
