using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Owner
    {
        public Owner()
        {
            ContactInfo = new HashSet<ContactInfo>();
            ContactLog = new HashSet<ContactLog>();
            InverseOwnerOwner = new HashSet<Owner>();
            OwnerDocuments = new HashSet<OwnerDocuments>();
            Ownership = new HashSet<Ownership>();
        }

        public Guid OwnerId { get; set; }
        public string PartyName { get; set; }
        public string OwnerType { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Guid? OwnerOwnerId { get; set; }
        public string OwnerAddress { get; set; }
        public bool IsDeleted { get; set; }

        public Owner OwnerOwner { get; set; }
        public ICollection<ContactInfo> ContactInfo { get; set; }
        public ICollection<ContactLog> ContactLog { get; set; }
        public ICollection<Owner> InverseOwnerOwner { get; set; }
        public ICollection<OwnerDocuments> OwnerDocuments { get; set; }
        public ICollection<Ownership> Ownership { get; set; }
    }
}
