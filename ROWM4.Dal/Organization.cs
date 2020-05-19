using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Organization
    {
        public Organization()
        {
            ContactInfo = new HashSet<ContactInfo>();
        }

        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<ContactInfo> ContactInfo { get; set; }
    }
}
