namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ContactLog", Schema ="ROWM")]
    public partial class ContactLog
    {
        public ContactLog()
        {
            ContactInfo = new HashSet<ContactInfo>();
            Parcel = new HashSet<Parcel>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactLogId { get; set; }

        public DateTimeOffset DateAdded { get; set; }

        public Guid ContactAgentId { get; set; }

        [StringLength(20)]
        public string ContactChannel { get; set; }

        [StringLength(50)]
        public string ProjectPhase { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public string Notes { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public Guid? Owner_OwnerId { get; set; }

        public int? Landowner_Score { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Owner Owner { get; set; }

        public virtual ICollection<ContactInfo> ContactInfo { get; set; }

        public virtual ICollection<Parcel> Parcel { get; set; }

        public virtual ICollection<Document> Attachments { get; set; }
    }
}
