using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class PreferredContactMode
    {
        public PreferredContactMode()
        {
            ContactInfo = new HashSet<ContactInfo>();
        }

        public string Mode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ContactInfo> ContactInfo { get; set; }
    }
}
