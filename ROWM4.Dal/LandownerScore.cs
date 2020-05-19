using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class LandownerScore
    {
        public int Score { get; set; }
        public int? DomainValue { get; set; }
        public string Caption { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
