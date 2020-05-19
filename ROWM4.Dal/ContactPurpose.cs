using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactPurpose
    {
        public string PurposeCode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string MilestoneCode { get; set; }

        public ParcelStatus MilestoneCodeNavigation { get; set; }
    }
}
