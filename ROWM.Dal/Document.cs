using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Document", Schema = "ROWM")]
    public class Document
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentId { get; set; }

        [StringLength(50)]
        public string DocumentType { get; set; }
        [StringLength(int.MaxValue)]
        public string Title { get; set; }
        [StringLength(500)]
        public string SourceFilename { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }
        [StringLength(int.MaxValue)]
        public string SharePointUrl { get; set; }   // readonly version for sharepoint accesss
        [StringLength(int.MaxValue)]
        public string AzureBlobId { get; set; } // eventually docs are to be in Azure storage


        // denormalized tracking
        public bool TitleInFile { get; set; }
        public DateTimeOffset? ReceivedDate { get; set; }
        public DateTimeOffset? QCDate { get; set; }     // ready for QC
        public DateTimeOffset? ApprovedDate { get; set; }   // QC approved
        public DateTimeOffset? SentDate { get; set; } // sent to owner
        [StringLength(100)]
        public string RowmTrackingNumber { get; set; }
        public DateTimeOffset? DeliveredDate { get; set; } // deliver to owner
        public DateTimeOffset? SignedDate { get; set; } // owner signed
        public DateTimeOffset? ReceivedFromOwnerDate { get; set; }
        [StringLength(100)]
        public string ClientTrackingNumber { get; set; }
        public DateTimeOffset? ClientSignatureDate { get; set; } // signed by client
        public DateTimeOffset? ReceivedFromClientDate { get; set; } // received from client

        public DateTimeOffset? DateRecorded { get; set; }
        public string CheckNo { get; set; }

        public virtual ICollection<Agent> Agents { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
        public virtual ICollection<Parcel> Parcel { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
