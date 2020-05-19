using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class RoeStatus
    {
        public RoeStatus()
        {
            Parcel = new HashSet<Parcel>();
        }

        public string Code { get; set; }
        public int DomainValue { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Parcel> Parcel { get; set; }
    }
}
