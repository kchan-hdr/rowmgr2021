namespace ROWM.Dal
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Parcel_Status", Schema ="ROWM")]
    public partial class Parcel_Status
    {
        public Parcel_Status()
        {
            Parcel = new HashSet<Parcel>();
        }

        [Key]
        [StringLength(40)]
        public string Code { get; set; }

        public string Category { get; set; }

        public int? DomainValue { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool? IsComplete { get; set; }
        public bool? IsAbort { get; set; }
        [StringLength(40)]
        public string ParentStatusCode { get; set; }

        public bool? IsRequired { get; set; }
        public bool? IsComputed { get; set; }
        public bool? ShowInPie { get; set; }

        public virtual ICollection<Parcel> Parcel { get; set; }

        [ForeignKey(nameof(Category))]
        public virtual StatusCategory StatusCategory { get; set; }
    }
}
