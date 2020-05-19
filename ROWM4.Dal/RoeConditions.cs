using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class RoeConditions
    {
        public Guid ConditionId { get; set; }
        public string Condition { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? EffectiveStartDate { get; set; }
        public DateTimeOffset? EffectiveEndDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Guid? ParcelParcelId { get; set; }
        public Guid? Contact { get; set; }

        public ContactInfo ContactNavigation { get; set; }
        public Parcel ParcelParcel { get; set; }
    }
}
