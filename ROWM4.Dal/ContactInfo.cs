using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactInfo
    {
        public ContactInfo()
        {
            ContactInfoContactLogs = new HashSet<ContactInfoContactLogs>();
            ParcelContactInfo = new HashSet<ParcelContactInfo>();
            RoeConditions = new HashSet<RoeConditions>();
        }

        public Guid ContactId { get; set; }
        public bool IsPrimaryContact { get; set; }
        public Guid ContactOwnerId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public string Representation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public Guid? OrganizationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool DoNotEmail { get; set; }
        public bool DoNotLetter { get; set; }
        public bool DoNotCallHomePhone { get; set; }
        public bool DoNotCallWorkPhone { get; set; }
        public bool DoNotCallCellPhone { get; set; }
        public string PreferredContactMode { get; set; }

        public Owner ContactOwner { get; set; }
        public Organization Organization { get; set; }
        public PreferredContactMode PreferredContactModeNavigation { get; set; }
        public ICollection<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        public ICollection<ParcelContactInfo> ParcelContactInfo { get; set; }
        public ICollection<RoeConditions> RoeConditions { get; set; }
    }
}
