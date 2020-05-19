using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ParcelDocuments
    {
        public Guid DocumentDocumentId { get; set; }
        public Guid ParcelParcelId { get; set; }

        public Document DocumentDocument { get; set; }
        public Parcel ParcelParcel { get; set; }
    }
}
