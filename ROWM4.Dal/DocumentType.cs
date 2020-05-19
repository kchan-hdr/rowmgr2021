using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            Document = new HashSet<Document>();
        }

        public string DocTypeName { get; set; }
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string MilestoneCode { get; set; }

        public ParcelStatus MilestoneCodeNavigation { get; set; }
        public ICollection<Document> Document { get; set; }
    }
}
