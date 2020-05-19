using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class StatusActivity
    {
        public Guid ActivityId { get; set; }
        public DateTimeOffset ActivityDate { get; set; }
        public Guid ParentParcelId { get; set; }
        public Guid AgentId { get; set; }
        public string ParcelStatusCode { get; set; }
        public string OrigianlParcelStatusCode { get; set; }
        public string RoeStatusCode { get; set; }
        public string OriginalRoeStatusCode { get; set; }
        public string Notes { get; set; }

        public Agent Agent { get; set; }
        public Parcel ParentParcel { get; set; }
    }
}
