using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Document
    {
        public Document()
        {
            DocumentActivityChildDocument = new HashSet<DocumentActivity>();
            DocumentActivityParentDocument = new HashSet<DocumentActivity>();
            DocumentAgents = new HashSet<DocumentAgents>();
            OwnerDocuments = new HashSet<OwnerDocuments>();
            ParcelDocuments = new HashSet<ParcelDocuments>();
        }

        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; }
        public byte[] Content { get; set; }
        public string SharePointUrl { get; set; }
        public string AzureBlobId { get; set; }
        public bool TitleInFile { get; set; }
        public DateTimeOffset? ReceivedDate { get; set; }
        public DateTimeOffset? Qcdate { get; set; }
        public DateTimeOffset? ApprovedDate { get; set; }
        public DateTimeOffset? SentDate { get; set; }
        public string RowmTrackingNumber { get; set; }
        public DateTimeOffset? DeliveredDate { get; set; }
        public DateTimeOffset? SignedDate { get; set; }
        public DateTimeOffset? ReceivedFromOwnerDate { get; set; }
        public string ClientTrackingNumber { get; set; }
        public DateTimeOffset? ClientSignatureDate { get; set; }
        public DateTimeOffset? ReceivedFromClientDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Guid? DocumentPackagePackageId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string SourceFilename { get; set; }
        public DateTimeOffset? DateRecorded { get; set; }
        public string CheckNo { get; set; }
        public Guid? ContactLogContactLogId { get; set; }
        public bool IsDeleted { get; set; }

        public ContactLog ContactLogContactLog { get; set; }
        public Document DocumentNavigation { get; set; }
        public DocumentPackage DocumentPackagePackage { get; set; }
        public DocumentType DocumentTypeNavigation { get; set; }
        public Document InverseDocumentNavigation { get; set; }
        public ICollection<DocumentActivity> DocumentActivityChildDocument { get; set; }
        public ICollection<DocumentActivity> DocumentActivityParentDocument { get; set; }
        public ICollection<DocumentAgents> DocumentAgents { get; set; }
        public ICollection<OwnerDocuments> OwnerDocuments { get; set; }
        public ICollection<ParcelDocuments> ParcelDocuments { get; set; }
    }
}
