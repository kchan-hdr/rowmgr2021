using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactChannel
    {
        public string ContactTypeCode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
