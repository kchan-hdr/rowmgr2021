using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ParcelContactLogs
    {
        public Guid ContactLogContactLogId { get; set; }
        public Guid ParcelParcelId { get; set; }

        public ContactLog ContactLogContactLog { get; set; }
        public Parcel ParcelParcel { get; set; }
    }
}
