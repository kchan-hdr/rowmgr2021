namespace ROWM.Dal
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Document_Type", Schema ="ROWM")]
    public partial class Document_Type
    {
        [Key]
        [StringLength(200)]
        public string DocTypeName { get; set; }

        public string Description { get; set; }

        [StringLength(400)]
        public string FolderPath { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }


        [ForeignKey("Milestone")]
        public string MilestoneCode { get; set; }
        public virtual Parcel_Status Milestone { get; set; }
    }
}
