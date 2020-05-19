using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ParcelStatus
    {
        public ParcelStatus()
        {
            ContactPurpose = new HashSet<ContactPurpose>();
            DocumentType = new HashSet<DocumentType>();
            InverseParentStatusCodeNavigation = new HashSet<ParcelStatus>();
            Parcel = new HashSet<Parcel>();
        }

        public string Code { get; set; }
        public int DomainValue { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool? IsComplete { get; set; }
        public bool? IsAbort { get; set; }
        public string ParentStatusCode { get; set; }

        public ParcelStatus ParentStatusCodeNavigation { get; set; }
        public ICollection<ContactPurpose> ContactPurpose { get; set; }
        public ICollection<DocumentType> DocumentType { get; set; }
        public ICollection<ParcelStatus> InverseParentStatusCodeNavigation { get; set; }
        public ICollection<Parcel> Parcel { get; set; }
    }
}
