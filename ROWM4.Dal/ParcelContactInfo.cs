using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ParcelContactInfo
    {
        public Guid ParcelParcelId { get; set; }
        public Guid ContactInfoContactId { get; set; }

        public ContactInfo ContactInfoContact { get; set; }
        public Parcel ParcelParcel { get; set; }
    }
}
