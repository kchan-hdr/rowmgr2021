namespace ROWM.Dal
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contact_Purpose", Schema ="ROWM")]
    public partial class Contact_Purpose
    {
        [Key]
        [StringLength(50)]
        public string PurposeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        [ForeignKey("Milestone")]
        public string MilestoneCode { get; set; }
        public virtual Parcel_Status Milestone { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
