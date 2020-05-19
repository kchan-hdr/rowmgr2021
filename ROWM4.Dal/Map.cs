using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Map
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public int LayerType { get; set; }
        public string AgsUrl { get; set; }
        public string LayerId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
