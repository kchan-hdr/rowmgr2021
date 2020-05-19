using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class RepresentationType
    {
        public string RelationTypeCode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
