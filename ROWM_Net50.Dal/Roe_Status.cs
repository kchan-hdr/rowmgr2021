namespace ROWM.Dal
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Roe_Status", Schema ="ROWM")]
    public partial class Roe_Status
    {
        public Roe_Status()
        {
            Parcel = new HashSet<Parcel>();
        }

        [Key]
        [StringLength(40)]
        public string Code { get; set; }

        public int DomainValue { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
