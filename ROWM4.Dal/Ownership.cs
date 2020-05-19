using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Ownership
    {
        public Guid OwnershipId { get; set; }
        public Guid? ParcelId { get; set; }
        public Guid OwnerId { get; set; }
        public int OwnershipT { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }

        public Owner Owner { get; set; }
        public Parcel Parcel { get; set; }
    }
}
