namespace ROWM.Dal
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contact_Channel", Schema ="ROWM")]
    public partial class Contact_Channel
    {
        [Key]
        [StringLength(20)]
        public string ContactTypeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
