namespace ROWM.Dal
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Representation_Type", Schema ="ROWM")]
    public partial class Repesentation_Type
    {
        [Key]
        [StringLength(20)]
        public string RelationTypeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
